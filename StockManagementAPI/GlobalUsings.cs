global using StockDesk.StockManagement.Model;
global using StockDesk.StockManagement.Events;
global using StockDesk.StockManagement.Commands;
global using StockDesk.StockManagementAPI.Mappers;
global using StockDesk.StockManagement.DataAccess;
global using StockDesk.Infrastructure.Messaging;
global using StockDesk.Infrastructure.Messaging.Configuration;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Serilog;
global using Microsoft.OpenApi.Models;
global using Microsoft.AspNetCore.Mvc;
global using System.Text.RegularExpressions;
global using Polly;