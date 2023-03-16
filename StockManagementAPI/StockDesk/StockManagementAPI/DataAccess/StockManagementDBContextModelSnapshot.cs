﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StockDesk.StockManagement.DataAccess;

#nullable disable

namespace StockDesk.StockManagementAPI.DataAccess
{
    [DbContext(typeof(StockManagementDBContext))]
    partial class StockManagementDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("StockDesk.StockManagement.Model.Stock", b =>
                {
                    b.Property<string>("Ticker")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CompanyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OwnerId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Industry")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MarketCap")
                        .HasColumnType("decimal(18, 4)");

                    b.HasKey("Ticker");

                    b.ToTable("Stock", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}