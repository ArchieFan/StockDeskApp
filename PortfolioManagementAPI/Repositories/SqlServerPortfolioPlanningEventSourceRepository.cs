﻿namespace StockDesk.PortfolioManagementAPI.Repositories;

using StockDesk.PortfolioManagementAPI.Repositories.Model;

public class SqlServerPortfolioPlanningEventSourceRepository : IEventSourceRepository<PortfolioPlanning>
{
    private static readonly JsonSerializerSettings _serializerSettings;
    private static readonly Dictionary<DateTime, string> _store = new Dictionary<DateTime, string>();
    private string _connectionString;

    static SqlServerPortfolioPlanningEventSourceRepository()
    {
        _serializerSettings = new JsonSerializerSettings();
        _serializerSettings.Formatting = Formatting.Indented;
        _serializerSettings.Converters.Add(new StringEnumConverter
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        });
    }

    public SqlServerPortfolioPlanningEventSourceRepository(string connectionString)
    {
        _connectionString = connectionString;

        // init db
        Log.Information("Initialize Database");

        Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(10, r => TimeSpan.FromSeconds(10), (ex, ts) => { Log.Error("Error connecting to DB. Retrying in 10 sec."); })
        .ExecuteAsync(InitializeDatabaseAsync)
        .Wait();
    }

    public async Task<PortfolioPlanning> GetByIdAsync(string aggregateId)
    {
        PortfolioPlanning planning = null;
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            // get aggregate
            await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, r => TimeSpan.FromSeconds(5), (ex, ts) =>
                    { Console.WriteLine("Error connecting to DB. Retrying in 5 sec."); })
                .ExecuteAsync(() => conn.OpenAsync());

            var aggregate = await conn
                   .QuerySingleOrDefaultAsync<Aggregate>(
                       "select * from PortfolioPlanning where Id = @Id",
                       new { Id = aggregateId });

            if (aggregate == null)
            {
                return null;
            }

            // get events
            IEnumerable<AggregateEvent> aggregateEvents = await conn
                .QueryAsync<AggregateEvent>(
                    "select * from PortfolioPlanningEvent where Id = @Id order by [Version];",
                    new { Id = aggregateId });

            List<Event> events = new List<Event>();
            foreach (var aggregateEvent in aggregateEvents)
            {
                events.Add(DeserializeEventData(aggregateEvent.MessageType, aggregateEvent.EventData));
            }
            planning = new PortfolioPlanning(DateTime.ParseExact(aggregateId, "yyyy-MM-dd", CultureInfo.InvariantCulture), events);
        }
        return planning;
    }

    public async Task SaveAsync(string aggregateId, int originalVersion, int newVersion, IEnumerable<Event> newEvents)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            // update eventstore
            await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, r => TimeSpan.FromSeconds(5), (ex, ts) =>
                    { Console.WriteLine("Error connecting to DB. Retrying in 5 sec."); })
                .ExecuteAsync(() => conn.OpenAsync());

            using (var transaction = conn.BeginTransaction())
            {
                // store aggregate
                int affectedRows = 0;
                var aggregate = await conn
                    .QuerySingleOrDefaultAsync<Aggregate>(
                        "select * from PortfolioPlanning where Id = @Id",
                        new { Id = aggregateId },
                        transaction);

                if (aggregate != null)
                {
                    // update existing aggregate
                    affectedRows = await conn.ExecuteAsync(
                        @"update PortfolioPlanning
                              set [CurrentVersion] = @NewVersion
                              where [Id] = @Id
                              and [CurrentVersion] = @CurrentVersion;",
                        new
                        {
                            Id = aggregateId,
                            NewVersion = newVersion,
                            CurrentVersion = originalVersion
                        },
                        transaction);
                }
                else
                {
                    // insert new aggregate
                    affectedRows = await conn.ExecuteAsync(
                        "insert PortfolioPlanning ([Id], [CurrentVersion]) values (@Id, @CurrentVersion)",
                        new { Id = aggregateId, CurrentVersion = newVersion },
                        transaction);
                }

                // check concurrency
                if (affectedRows == 0)
                {
                    transaction.Rollback();
                    throw new ConcurrencyException();
                }

                // store events
                int eventVersion = originalVersion;
                foreach (var e in newEvents)
                {
                    eventVersion++;
                    await conn.ExecuteAsync(
                        @"insert PortfolioPlanningEvent ([Id], [Version], [Timestamp], [MessageType], [EventData])
                              values (@Id, @NewVersion, @Timestamp, @MessageType,@EventData);",
                        new
                        {
                            Id = aggregateId,
                            NewVersion = eventVersion,
                            Timestamp = DateTime.Now,
                            MessageType = e.MessageType,
                            EventData = SerializeEventData(e)
                        }, transaction);
                }

                // commit
                transaction.Commit();
            }
        }
    }

    private async Task InitializeDatabaseAsync()
    {
        // init db
        using (SqlConnection conn = new SqlConnection(_connectionString.Replace("PortfolioManagementEventStore", "master")))
        {
            await conn.OpenAsync();

            // create database
            string sql = "IF NOT EXISTS(SELECT * FROM master.sys.databases WHERE name='PortfolioManagementEventStore') CREATE DATABASE PortfolioManagementEventStore;";
            conn.Execute(sql);
        }

        // create tables
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            string sql = @" 
                    if OBJECT_ID('PortfolioPlanning') IS NULL 
                    CREATE TABLE PortfolioPlanning (
                        [Id] varchar(50) NOT NULL,
                        [CurrentVersion] int NOT NULL,
                    PRIMARY KEY([Id]));
                   
                    if OBJECT_ID('PortfolioPlanningEvent') IS NULL
                    CREATE TABLE PortfolioPlanningEvent (
                        [Id] varchar(50) NOT NULL REFERENCES PortfolioPlanning([Id]),
                        [Version] int NOT NULL,
                        [Timestamp] datetime2(7) NOT NULL,
                        [MessageType] varchar(75) NOT NULL,
                        [EventData] text,
                    PRIMARY KEY([Id], [Version]));";
            conn.Execute(sql);
        }
    }

    /// <summary>
    /// Get events for a certain aggregate.
    /// </summary>
    /// <param name="planningId">The id of the planning.</param>
    /// <param name="conn">The SQL connection to use.</param>
    /// <returns></returns>
    private async Task<IEnumerable<Event>> GetAggregateEvents(string planningId, SqlConnection conn)
    {
        IEnumerable<AggregateEvent> aggregateEvents = await conn
            .QueryAsync<AggregateEvent>(
                "select * from PortfolioPlanningEvent where Id = @Id order by [Version]",
                new { Id = planningId });

        List<Event> events = new List<Event>();
        foreach (var aggregateEvent in aggregateEvents)
        {
            events.Add(DeserializeEventData(aggregateEvent.MessageType, aggregateEvent.EventData));
        }
        return events;
    }

    /// <summary>
    /// Serialize event-data to JSON.
    /// </summary>
    /// <param name="eventData">The event-data to serialize.</param>
    private string SerializeEventData(Event eventData)
    {
        return JsonConvert.SerializeObject(eventData, _serializerSettings);
    }

    /// <summary>
    /// Deserialize event-data from JSON.
    /// </summary>
    /// <param name="messageType">The message-type of the event.</param>
    /// <param name="eventData">The event-data JSON to deserialize.</param>
    private Event DeserializeEventData(string messageType, string eventData)
    {
        Type eventType = Type.GetType($"StockDesk.PortfolioManagementAPI.Events.{messageType}");
        JObject obj = JsonConvert.DeserializeObject<JObject>(eventData, _serializerSettings);
        return obj.ToObject(eventType) as Event;
    }
}