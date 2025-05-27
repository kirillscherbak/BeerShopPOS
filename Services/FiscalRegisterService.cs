using System.IO.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BeerShopPOS.Models;

namespace BeerShopPOS.Services
{
    public class FiscalRegisterService : BaseService, IFiscalRegisterService
    {
        private readonly SerialPort _serialPort;
        private readonly string _driverType;
        private bool _isInitialized;
        private string _currentCashier = "";

        public FiscalRegisterService(
            IConfiguration configuration, 
            ILogger<FiscalRegisterService> logger) : base(logger)
        {
            var config = configuration.GetSection("Fiscal");
            _driverType = config["DriverType"] ?? "Atol";

            _serialPort = new SerialPort
            {
                PortName = config["ComPort"] ?? "COM1",
                BaudRate = 115200,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None
            };
        }

        public async Task OpenShiftAsync(string cashierName)
        {
            await ExecuteWithLoggingAsync(async () =>
            {
                InitializeIfNeeded();
                _currentCashier = cashierName;

                // Simulate opening shift
                await Task.Delay(1000);

                LogInfo($"Смена открыта кассиром {cashierName}");
                return true;
            }, $"Открытие смены кассиром {cashierName}");
        }

        public async Task CloseShiftAsync()
        {
            await ExecuteWithLoggingAsync(async () =>
            {
                InitializeIfNeeded();

                // Simulate closing shift
                await Task.Delay(1000);

                LogInfo($"Смена закрыта кассиром {_currentCashier}");
                _currentCashier = "";
                return true;
            }, "Закрытие смены");
        }

        public async Task<string> PrintReceiptAsync(Receipt receipt)
        {
            return await ExecuteWithLoggingAsync(async () =>
            {
                InitializeIfNeeded();

                // Generate fiscal number (in real implementation this would come from the fiscal device)
                var fiscalNumber = DateTime.Now.ToString("yyyyMMddHHmmss");

                // Simulate printing receipt
                await Task.Delay(2000);

                LogInfo($"Чек №{fiscalNumber} распечатан");
                return fiscalNumber;
            }, $"Печать чека {receipt.Id}");
        }

        public async Task VoidReceiptAsync(Receipt receipt, string reason)
        {
            await ExecuteWithLoggingAsync(async () =>
            {
                InitializeIfNeeded();

                // Simulate voiding receipt
                await Task.Delay(1000);

                LogInfo($"Чек №{receipt.FiscalNumber} аннулирован. Причина: {reason}");
                return true;
            }, $"Аннулирование чека {receipt.FiscalNumber}");
        }

        private void InitializeIfNeeded()
        {
            if (!_isInitialized)
            {
                try
                {
                    _serialPort.Open();
                    _isInitialized = true;
                    LogInfo($"ККМ {_driverType} инициализирована");
                }
                catch (Exception ex)
                {
                    LogError(ex, "Ошибка инициализации ККМ");
                    throw;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_isInitialized)
                {
                    _serialPort.Close();
                }
                _serialPort.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}