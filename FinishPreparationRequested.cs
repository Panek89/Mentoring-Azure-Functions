using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace Mentoring.Function;

public class FinishPreparationRequested
{
    private readonly ILogger<FinishPreparationRequested> _logger;

    public FinishPreparationRequested(ILogger<FinishPreparationRequested> logger)
    {
        _logger = logger;
    }

    [Function(nameof(FinishPreparationRequested))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "finish/{instanceId}")] HttpRequest req, string instanceId, [DurableClient] DurableTaskClient client)
    {
        await client.RaiseEventAsync(instanceId, nameof(FinishPreparationRequested), "finish");

        return new OkResult();
    }
}