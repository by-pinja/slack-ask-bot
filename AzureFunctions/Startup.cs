using System.Diagnostics;
using System.Reflection;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using AzureFunctions;
using Microsoft.Extensions.Configuration;
using CloudLib;
using SlackLib;

[assembly: WebJobsStartup(typeof(Startup))]
namespace AzureFunctions
{
    internal class CustomTelemetryInitializer : ITelemetryInitializer
    {
        private readonly string _version;

        public CustomTelemetryInitializer()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            _version = fileVersionInfo.ProductVersion;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Component.Version = _version;
        }
    }

    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var tableStorageSettings = config.GetSection("TableStorage").Get<TableStorageSettings>();
            var slackClientSettings = config.GetSection("SlackClient").Get<SlackClientSettings>();

            builder.Services.AddSingleton<ITelemetryInitializer, CustomTelemetryInitializer>();
            builder.Services.AddSingleton(tableStorageSettings);
            builder.Services.AddSingleton(slackClientSettings);
            builder.Services.AddTransient<PayloadParser>();
            builder.Services.AddSingleton<IStorage, Storage>();
            builder.Services.AddTransient<SlackResponseParser>();
            builder.Services.AddTransient<SlackClient>();
            builder.Services.AddLogging();
        }
    }
}