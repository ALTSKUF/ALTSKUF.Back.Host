
using Microsoft.AspNetCore.Mvc;
using AltSKUF.WebApi.Authorization;

namespace AltSKUF.WebApi.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class AuthorizationController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _userUrl;
        private readonly ILogger _logger;
        public AuthorizationController(HttpClient httpClient, IConfiguration configuration, ILogger<AuthorizationController> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _userUrl = _configuration["UserUrl"]!;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserEmailRegistrationRequest registrationRequest)
        {
            string registrationServiceUrl = $"{_userUrl}/Auth/Email"; 

            var response = await _httpClient.PostAsJsonAsync(registrationServiceUrl, registrationRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error occurred while calling the registration service.");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser([FromBody] UserEmailAuthRequest authRequest)
        {
            string authServiceUrl = $"{_userUrl}/Auth/Email";

            var response = await _httpClient.PostAsJsonAsync(authServiceUrl, authRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error occurred while calling the authentication service.");
            }
        }

        [HttpGet("VerifyFromEmail")]
        public async Task<IActionResult> VerifyFromEmail([FromQuery] Guid userId, [FromQuery] string code)
        {
            string baseUrl = _userUrl.TrimEnd('/');
            string fullPath = $"{baseUrl}/Auth/Email/Verify/{userId}?code={Uri.EscapeDataString(code)}";

            _logger.LogInformation($"Отправка верификационного запроса: {fullPath}");

            try
            {
                var response = await _httpClient.GetAsync(fullPath);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Ошибка верификации. Status: {response.StatusCode}, Response: {errorContent}");
                return StatusCode((int)response.StatusCode, errorContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка прокси при верификации");
                return StatusCode(500, new { Message = "Ошибка при обращении к сервису верификации" });
            }
        }
    }
}