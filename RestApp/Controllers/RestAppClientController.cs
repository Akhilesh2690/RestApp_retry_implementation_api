using Microsoft.AspNetCore.Mvc;
using RestApp.Helper;
using RestApp.Model;
using System.Net;

namespace RestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestAppClientController : ControllerBase
    {
        private readonly IRestClient _restClient;
        private readonly ILogger<RestAppClientController> _logger;

        public RestAppClientController(IRestClient restClient, ILogger<RestAppClientController> logger)
        {
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.LogInformation("ServiceController started");
        }

        [HttpGet("get-with-retry")]
        public async Task<ActionResult> GetRetryAsync(string url)
        {
            try
            {
                var result = await RetryHelper.ExecuteWithRetryAsync<string>(
                    async () => await _restClient.Get(url),
                    maxRetries: 3,
                    retryDelay: TimeSpan.FromSeconds(10),
                    exceptionTypeToHandle: typeof(WebException),
                    logger: _logger
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching data");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}