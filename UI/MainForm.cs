public partial class MainForm : Form
{
    private readonly PosService _posService;
    private readonly BarcodeScannerService _barcodeService;
    
    public MainForm(PosService posService, BarcodeScannerService barcodeService)
    {
        _posService = posService;
        _barcodeService = barcodeService;
        
        InitializeComponent();
        
        _barcodeService.BarcodeScanned += OnBarcodeScanned;
        _posService.StartNewReceipt();
    }

    private void OnBarcodeScanned(object sender, string barcode)
    {
        this.Invoke((MethodInvoker)delegate 
        {
            var product = _barcodeService.GetProductByBarcode(barcode);
            if (product != null)
            {
                _posService.AddProductToReceipt(product);
                UpdateReceiptDisplay();
                PlaySuccessSound();
            }
            else
            {
                MessageBox.Show($"Товар с кодом {barcode} не найден");
                PlayErrorSound();
            }
        });
    }

    private void UpdateReceiptDisplay()
    {
        receiptItemsGrid.DataSource = null;
        receiptItemsGrid.DataSource = _posService.CurrentReceipt.Items;
        totalLabel.Text = $"Итого: {_posService.CurrentReceipt.TotalAmount} руб.";
    }

    private async void completeSaleButton_Click(object sender, EventArgs e)
    {
        try
        {
            var paymentType = cashRadio.Checked ? PaymentType.Cash : PaymentType.Card;
            var receipt = await _posService.CompleteReceiptAsync(paymentType);
            
            MessageBox.Show($"Чек №{receipt.FiscalNumber} успешно зарегистрирован");
            _posService.StartNewReceipt();
            UpdateReceiptDisplay();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}");
        }
    }

    private void PlaySuccessSound() => SystemSounds.Beep.Play();
    private void PlayErrorSound() => SystemSounds.Hand.Play();
}