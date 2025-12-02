using Azure.Data.AppConfiguration;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = FunctionsApplication.CreateBuilder(args);

builder.Configuration.AddAzureAppConfiguration(options =>
{
    var envVar = Environment.GetEnvironmentVariable("MENTORINGFUNCTIONS_APP_CONFIG");
    options.Connect(new Uri(Environment.GetEnvironmentVariable("MENTORINGFUNCTIONS_APP_CONFIG")!), new DefaultAzureCredential())
                .Select("Mentoring:*", LabelFilter.Null);
});

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton(sp =>
{
    return new ConfigurationClient(
        new Uri(Environment.GetEnvironmentVariable("MENTORINGFUNCTIONS_APP_CONFIG")!),
        new DefaultAzureCredential());
});


builder.Build().Run();
