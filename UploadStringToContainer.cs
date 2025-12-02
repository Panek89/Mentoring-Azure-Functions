using Azure.Data.AppConfiguration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace Mentoring.Function;

public class UploadStringToContainer
{
    private readonly ConfigurationClient _client;

    private readonly ILogger<UploadStringToContainer> _logger;

    public UploadStringToContainer(IConfiguration configuration, ConfigurationClient client, ILogger<UploadStringToContainer> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Function("UploadStringToContainer")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, [FromBody] InputValue inputValue)
    {
        await _client.SetConfigurationSettingAsync("Mentoring:functionInputValue", inputValue.Value);
        _logger.LogInformation($"Input value: {inputValue}");

        return new OkObjectResult("Saved");
    }

    public record InputValue(string Value);
}