IHost host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.UseRabbitMQMessageHandler(hostContext.Configuration);

        services.AddTransient<PortfolioManagementDBContext>((svc) =>
        {
            var sqlConnectionString = hostContext.Configuration.GetConnectionString("PortfolioManagementCN");
            var dbContextOptions = new DbContextOptionsBuilder<PortfolioManagementDBContext>()
                .UseSqlServer(sqlConnectionString)
                .Options;
            var dbContext = new PortfolioManagementDBContext(dbContextOptions);

            DBInitializer.Initialize(dbContext);

            return dbContext;
        });

        services.AddHostedService<EventHandlerWorker>();
    })
    .UseSerilog((hostContext, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
    })
    .UseConsoleLifetime()
    .Build();

await host.RunAsync();