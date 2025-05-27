using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BeerShopPOS.Models;

namespace BeerShopPOS.Services
{
    public class EGAISService : BaseService, IEGAISService
    {
        private readonly string _apiUrl;
        private readonly string _login;
        private readonly string _password;
        private readonly HttpClient _httpClient;

        public EGAISService(
            IConfiguration configuration, 
            ILogger<EGAISService> logger) : base(logger)
        {
            var config = configuration.GetSection("EGAIS");
            _apiUrl = config["ApiUrl"] ?? throw new ArgumentException("EGAIS ApiUrl not configured");
            _login = config["Login"] ?? throw new ArgumentException("EGAIS Login not configured");
            _password = config["Password"] ?? throw new ArgumentException("EGAIS Password not configured");

            _httpClient = new HttpClient { BaseAddress = new Uri(_apiUrl) };
            _httpClient.DefaultRequestHeaders.Add("Authorization", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_login}:{_password}")));
        }

        public async Task<bool> ValidateProductAsync(Product product)
        {
            return await ExecuteWithLoggingAsync(async () =>
            {
                // In real implementation, this would call EGAIS API
                // Simulating API call
                await Task.Delay(500);

                if (string.IsNullOrEmpty(product.EGAISCode))
                {
                    LogWarning($"Товар {product.Name} не имеет кода ЕГАИС");
                    return false;
                }

                LogInfo($"Товар {product.Name} прошел проверку ЕГАИС");
                return true;
            }, $"Проверка товара {product.Name} в ЕГАИС");
        }

        public async Task<string> RegisterSaleAsync(Receipt receipt)
        {
            return await ExecuteWithLoggingAsync(async () =>
            {
                // In real implementation, this would call EGAIS API
                // Simulating API call
                await Task.Delay(1000);

                var checkNumber = $"EGAIS-{DateTime.Now:yyyyMMddHHmmss}";
                LogInfo($"Продажа зарегистрирована в ЕГАИС, номер чека: {checkNumber}");
                return checkNumber;
            }, $"Регистрация продажи в ЕГАИС");
        }

        public async Task<bool> CancelSaleAsync(string egaisCheckNumber)
        {
            return await ExecuteWithLoggingAsync(async () =>
            {
                // In real implementation, this would call EGAIS API
                // Simulating API call
                await Task.Delay(1000);

                LogInfo($"Продажа {egaisCheckNumber} отменена в ЕГАИС");
                return true;
            }, $"Отмена продажи {egaisCheckNumber} в ЕГАИС");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}