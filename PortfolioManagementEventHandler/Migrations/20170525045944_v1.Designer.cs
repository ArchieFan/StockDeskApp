using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using StockDesk.PortfolioManagementEventHandler.DataAccess;

namespace StockDesk.PortfolioManagementEventHandler.Migrations
{
    [DbContext(typeof(PortfolioManagementDBContext))]
    [Migration("20170525045944_v1")]
    partial class v1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("StockDesk.PortfolioManagementEventHandler.Client", b =>
                {
                    b.Property<string>("ClientId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("TelephoneNumber");

                    b.HasKey("ClientId");

                    b.ToTable("Client");
                });

            modelBuilder.Entity("StockDesk.PortfolioManagementEventHandler.Model.Trading", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("ActualEndTime");

                    b.Property<DateTime?>("ActualStartTime");

                    b.Property<string>("ClientId");

                    b.Property<string>("Description");

                    b.Property<DateTime>("EndTime");

                    b.Property<string>("Notes");

                    b.Property<DateTime>("StartTime");

                    b.Property<string>("StockTicker");

                    b.Property<DateTime>("PortfolioPlanningDate");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("StockTicker");

                    b.ToTable("Trading");
                });

            modelBuilder.Entity("StockDesk.PortfolioManagementEventHandler.Model.Stock", b =>
                {
                    b.Property<string>("Ticker")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CompanyName");

                    b.Property<string>("OwnerId");

                    b.Property<string>("Industry");

                    b.Property<decimal>("MarketCap");

                    b.HasKey("Ticker");

                    b.ToTable("Stock");
                });

            modelBuilder.Entity("StockDesk.PortfolioManagementEventHandler.Model.Trading", b =>
                {
                    b.HasOne("StockDesk.PortfolioManagementEventHandler.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId");

                    b.HasOne("StockDesk.PortfolioManagementEventHandler.Model.Stock", "Stock")
                        .WithMany()
                        .HasForeignKey("StockTicker");
                });
        }
    }
}
