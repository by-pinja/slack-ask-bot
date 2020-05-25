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
using AskBotCore;
using System;

[assembly: WebJobsStartup(typeof(Startup))]
namespace AzureFunctions
{
    internal class CustomTelemetryInitializer : ITelemetryInitializer
    {
        private readonly string _version;

        public CustomTelemetryInitializer()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
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
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var tableStorageSettings = config.GetSection("TableStorage").Get<TableStorageSettings>();
            var slackClientSettings = config.GetSection("SlackClient").Get<SlackClientSettings>();

            builder.Services.AddHttpClient<ISlackClient, SlackClient>(c =>
            {
                c.BaseAddress = new Uri("https://slack.com/api/");
                c.DefaultRequestHeaders.Add("Authorization", $"Bearer {slackClientSettings.BearerToken}");
            });

            builder.Services.AddSingleton<ITelemetryInitializer, CustomTelemetryInitializer>()
            .AddSingleton(tableStorageSettings)
            .AddSingleton(slackClientSettings)
            .AddTransient<IStorage, Storage>()
            .AddTransient<IAskBotControl, AskBotControl>()
            .AddLogging();
        }
    }
}