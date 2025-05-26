public class BarcodeScannerService
{
    private readonly ProductService _productService;
    private readonly SerialPort _serialPort;
    
    public event EventHandler<string> BarcodeScanned;

    public BarcodeScannerService(ProductService productService, IConfiguration config)
    {
        _productService = productService;
        
        // Настройка COM-порта для сканера
        _serialPort = new SerialPort(
            config["BarcodeScanner:ComPort"],
            config["BarcodeScanner:BaudRate"]);
        
        _serialPort.DataReceived += SerialPort_DataReceived;
        _serialPort.Open();
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var barcode = _serialPort.ReadLine().Trim();
        BarcodeScanned?.Invoke(this, barcode);
    }

    public Product GetProductByBarcode(string barcode)
    {
        return _productService.GetProductByBarcode(barcode);
    }
}