using System.Text;
using Mentoring.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace Mentoring.Function;

public static class FileProcessingDurableMentoring
{
    [Function(nameof(FileProcessingDurableMentoring))]
    public static async Task<string> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger logger = context.CreateReplaySafeLogger(nameof(FileProcessingDurableMentoring));
        var fileContent = context.GetInput<string>();
        context.SetCustomStatus("Starting initial processing");

        var whyWorry = await context.WaitForExternalEvent<string>(nameof(HttpTriggerMentoring));

        var generateStringTasks = new List<Task<string>>();
        for (int i = 0; i < 3; i++)
        {
            generateStringTasks.Add(context.CallActivityAsync<string>(nameof(GenerateRandomStringActivity), string.Empty));
        }

        var results = await Task.WhenAll(generateStringTasks);
        fileContent += results.OrderByDescending(s => s.Length).First();

        var eventData = await context.WaitForExternalEvent<string>(nameof(FinishPreparationRequested));

        await context.CallActivityAsync(nameof(FileSaveToBlobActivity), fileContent);

        context.SetCustomStatus("Processing finished");
        return $"Processing completed for {fileContent}";
    }

    [Function(nameof(GenerateRandomStringActivity))]
    public static string GenerateRandomStringActivity([ActivityTrigger] string input, FunctionContext context)
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

        var logger = context.GetLogger("GenerateRandomStringActivity");
        logger.LogInformation("Activity generated random string.");
        return randomString.ToString();
    }
}