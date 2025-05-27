using System.IO.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BeerShopPOS.Models;

namespace BeerShopPOS.Services
{
    public class BarcodeScannerService : BaseService, IBarcodeScannerService
    {
        private readonly IProductService _productService;
        private readonly SerialPort _serialPort;
        private bool _isInitialized;
        private string? _lastScannedBarcode;
        private readonly SemaphoreSlim _serialPortLock = new(1, 1);
        
        public event AsyncEventHandler<string>? BarcodeScanned;

        public BarcodeScannerService(
            IProductService productService, 
            IConfiguration config, 
            ILogger<BarcodeScannerService> logger) : base(logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            
            var portConfig = config.GetSection("BarcodeScanner");
            _serialPort = new SerialPort
            {
                PortName = portConfig["ComPort"] ?? "COM1",
                BaudRate = int.Parse(portConfig["BaudRate"] ?? "9600"),
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
                ReadTimeout = 1000, // 1 second timeout
                WriteTimeout = 1000
            };
            
            _serialPort.DataReceived += SerialPort_DataReceived;
        }

        public Task StartScanningAsync()
        {
            return ExecuteWithLoggingAsync(() =>
            {
                if (!_isInitialized)
                {
                    _serialPort.Open();
                    _isInitialized = true;
                }
                return Task.FromResult(true);
            }, "Запуск сканера штрихкодов");
        }

        public Task StopScanningAsync()
        {
            return ExecuteWithLoggingAsync(() =>
            {
                if (_isInitialized)
                {
                    _serialPort.Close();
                    _isInitialized = false;
                }
                return Task.FromResult(true);
            }, "Остановка сканера штрихкодов");
        }

        public string? GetLastScannedBarcode()
        {
            return _lastScannedBarcode;
        }

        private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // Take a lock to ensure thread-safe access to the serial port
                await _serialPortLock.WaitAsync();
                try
                {
                    var barcode = await Task.Run(() => _serialPort.ReadLine().Trim());
                    _lastScannedBarcode = barcode;
                    LogInfo($"Считан штрихкод: {barcode}");
                    
                    if (BarcodeScanned != null)
                    {
                        // Fire event handlers asynchronously to avoid blocking the serial port
                        foreach (AsyncEventHandler<string> handler in BarcodeScanned.GetInvocationList())
                        {
                            try
                            {
                                await handler(this, barcode);
                            }
                            catch (Exception handlerEx)
                            {
                                LogError(handlerEx, "Ошибка в обработчике события BarcodeScanned");
                            }
                        }
                    }
                }
                finally
                {
                    _serialPortLock.Release();
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Ошибка при чтении штрихкода");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _serialPortLock.Dispose();
                if (_isInitialized)
                {
                    _serialPort.Close();
                }
                _serialPort.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // Define AsyncEventHandler delegate for async event pattern
    public delegate Task AsyncEventHandler<TEventArgs>(object sender, TEventArgs e);
}