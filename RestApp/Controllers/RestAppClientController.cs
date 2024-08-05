using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        private readonly RetrySettings _retrySettings;

        public RestAppClientController(IRestClient restClient, ILogger<RestAppClientController> logger, IOptions<RetrySettings> retrySettings)
        {
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retrySettings = retrySettings.Value;
            _logger.LogInformation("RestAppClientController started");
        }

        /// <summary>
        /// Requests data from a specified url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        ///
        [HttpGet("get-with-retry")]
        public async Task<ActionResult> GetRetryAsync(string url)
        {
            try
            {
                var result = await RetryHelper.ExecuteWithRetryAsync<string>(
                    async () => await _restClient.GetDataFromUrl(url),
                    maxRetries: _retrySettings.MaxRetries,
                    retryDelay: TimeSpan.FromSeconds(_retrySettings.RetryDelaySeconds),
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