﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using Serilog.Events;
using System.Collections.ObjectModel;

namespace APIGateway
{
    public class Program
    {
        public static readonly string AppName = typeof(Program).Namespace;


        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();

            CreateSerilogLogger(configuration);

            new WebHostBuilder()
                .UseKestrel()
               .UseContentRoot(Directory.GetCurrentDirectory())
               .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   config
                       .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                       .AddJsonFile("appsettings.json", true, true)
                       .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                       .AddJsonFile("configuration.json", false, true)
                       .AddEnvironmentVariables();
               })
               .UseIISIntegration()
               .UseStartup<Startup>()
               .UseSerilog()
               .Build()
               .Run();

            //IWebHostBuilder builder = new WebHostBuilder();
            //builder.ConfigureServices(s =>
            //{
            //    s.AddSingleton(builder);
            //});
            //builder.UseKestrel()
            //       .UseContentRoot(Directory.GetCurrentDirectory())
            //       .UseStartup<Startup>()
            //       .UseUrls("http://localhost:5000") // TODO: need?
            //       .UseSerilog(); 

            //var host = builder.Build();
            //host.Run();
        }

        public static void CreateSerilogLogger(IConfiguration configuration)
        {
            var columnOptions = new ColumnOptions();
            // we don't need XML data
            columnOptions.Store.Remove(StandardColumn.Properties);
            columnOptions.Store.Remove(StandardColumn.Id);
            columnOptions.Store.Remove(StandardColumn.MessageTemplate);
            // we do want JSON data
            columnOptions.Store.Add(StandardColumn.LogEvent);
            columnOptions.AdditionalColumns = new Collection<SqlColumn>()
            {
                 new SqlColumn("ApplicationContext", System.Data.SqlDbType.NVarChar, dataLength: 25),
                new SqlColumn("CorrelationID", System.Data.SqlDbType.NVarChar, dataLength:36),
                new SqlColumn("Username", System.Data.SqlDbType.NVarChar, dataLength: 50)
            };
            columnOptions.LogEvent.ExcludeAdditionalProperties = true;
            columnOptions.LogEvent.ExcludeStandardColumns = true;
            var logDb = configuration.GetConnectionString("LogDb");
            var logTable = "Log";

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Ocelot", LogEventLevel.Information)
            .Enrich.WithProperty("ApplicationContext", AppName)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} CorrID: {CorrelationID}] {Message}{NewLine}{Exception}")
            .WriteTo.MSSqlServer(
                autoCreateSqlTable: true,
                connectionString: logDb,
                tableName: logTable,
                columnOptions: columnOptions
            )
            .CreateLogger();
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
