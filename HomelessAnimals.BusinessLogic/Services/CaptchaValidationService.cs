using HomelessAnimals.BusinessLogic.Interfaces;
using HomelessAnimals.BusinessLogic.Models;
using HomelessAnimals.Shared.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace HomelessAnimals.BusinessLogic.Services
{
    public class CaptchaValidationService : ICaptchaValidationService
    {
        private readonly HttpClient _httpClient;
        private readonly ReCaptchaSettings _captchaSettings;

        public CaptchaValidationService(HttpClient httpClient, IOptions<ReCaptchaSettings> captchaSettings)
        {
            _httpClient = httpClient;
            _captchaSettings = captchaSettings.Value;
        }

        public async Task<bool> IsHuman(string token)
        {
            var payload = new Dictionary<string, string>
            {
                ["secret"] = _captchaSettings.SecretKey,
                ["response"] = token
            };

            var response = await _httpClient.PostAsync(string.Empty, new FormUrlEncodedContent(payload));
            var result = await response.Content.ReadFromJsonAsync<CaptchaValidationResult>();

            return result.Success && result.Score > 0.5;
        }
    }
}
