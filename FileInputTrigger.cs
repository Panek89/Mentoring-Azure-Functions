using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask;

namespace Mentoring.Function;

public class FileInputTrigger
{
    private readonly ILogger<FileInputTrigger> _logger;

    public FileInputTrigger(ILogger<FileInputTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(FileInputTrigger))]
    public async Task Run([BlobTrigger("durable-blob/tekst.txt")] Stream stream, string name, [DurableClient] DurableTaskClient client)
    {
        using var blobStreamReader = new StreamReader(stream);
        var content = await blobStreamReader.ReadToEndAsync();
        _logger.LogInformation("C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}", name, content);

        string instanceId = $"file-processing-{name}";

        await client.ScheduleNewOrchestrationInstanceAsync(nameof(FileProcessingDurableMentoring), content, options: new StartOrchestrationOptions { InstanceId = instanceId });
    }
}