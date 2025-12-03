using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace Mentoring.Function;

public class HttpTriggerMentoring
{
    private readonly ILogger<HttpTriggerMentoring> _logger;

    public HttpTriggerMentoring(ILogger<HttpTriggerMentoring> logger)
    {
        _logger = logger;
    }

    [Function(nameof(HttpTriggerMentoring))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "generate-text")] HttpRequest req, [DurableClient] DurableTaskClient client)
    {
        var random = new Random();
        var randomString = new StringBuilder();
        var simpleAlphabet = "abcdefghijklmnopqrstuvwxyz";
        var numberOfRandom = random.Next(1, 100);

        for (int i = 0; i < numberOfRandom; i++)
        {
            int x = random.Next(26);
            randomString.Append(simpleAlphabet[x]);
        }

        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string? instanceId = req.Query["instanceId"];
        if (!string.IsNullOrEmpty(instanceId))
        {
            await client.RaiseEventAsync(instanceId, nameof(HttpTriggerMentoring), "finish");
        }

        return new OkObjectResult($"{randomString}");
    }
}