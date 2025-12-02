using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Mantoring.Function;

public class HttpTriggerMentoring
{
    private readonly ILogger<HttpTriggerMentoring> _logger;

    public HttpTriggerMentoring(ILogger<HttpTriggerMentoring> logger)
    {
        _logger = logger;
    }

    [Function("HttpTriggerMentoring")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
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
        return new OkObjectResult($"{randomString}");
    }
}