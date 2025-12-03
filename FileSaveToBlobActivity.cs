using System;
using System.Text;
using Azure.Data.AppConfiguration;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Mentoring.Functions;

public class FileSaveToBlobActivity
{
    private readonly ILogger _logger;

    public FileSaveToBlobActivity(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<FileSaveToBlobActivity>();
    }

    [Function(nameof(FileSaveToBlobActivity))]
    public async Task Run([ActivityTrigger] string content)
    {
        _logger.LogInformation("Saving content to Blob Storage");

        var blobUri = new Uri($"");

        string containerName = "durable-blob";
        string blobName = $"result-{Guid.NewGuid()}.txt";

        var blobServiceClient = new BlobServiceClient(blobUri);
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(blobName);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await blobClient.UploadAsync(stream, overwrite: true);

        _logger.LogInformation($"Saved blob: {blobName}");
    }

}
