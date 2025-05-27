using System.IO.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BeerShopPOS.Services
{
    public class PaymentTerminalService : BaseService, IPaymentTerminalService
    {
        private readonly SerialPort _serialPort;
        private readonly IConfiguration _configuration;
        private bool _isInitialized;

        public PaymentTerminalService(IConfiguration configuration, ILogger<PaymentTerminalService> logger)
            : base(logger)
        {
            _configuration = configuration;
            var config = _configuration.GetSection("PaymentTerminal");
            
            _serialPort = new SerialPort
            {
                PortName = config["ComPort"] ?? "COM1",
                BaudRate = 9600,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None
            };
        }

        public async Task<bool> ProcessPaymentAsync(decimal amount)
        {
            return await ExecuteWithLoggingAsync(async () =>
            {
                InitializeIfNeeded();

                // Simulate payment processing
                await Task.Delay(2000); // Simulated delay for payment processing

                LogInfo($"Оплата на сумму {amount:C} успешно проведена");
                return true;
            }, $"Проведение оплаты на сумму {amount:C}");
        }

        public async Task<bool> VoidPaymentAsync(decimal amount, string transactionId)
        {
            return await ExecuteWithLoggingAsync(async () =>
            {
                InitializeIfNeeded();

                // Simulate void operation
                await Task.Delay(1000); // Simulated delay for void operation

                LogInfo($"Возврат платежа на сумму {amount:C} выполнен успешно");
                return true;
            }, $"Возврат платежа на сумму {amount:C}");
        }

        private void InitializeIfNeeded()
        {
            if (!_isInitialized)
            {
                try
                {
                    _serialPort.Open();
                    _isInitialized = true;
                    LogInfo("Платежный терминал инициализирован");
                }
                catch (Exception ex)
                {
                    LogError(ex, "Ошибка инициализации платежного терминала");
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