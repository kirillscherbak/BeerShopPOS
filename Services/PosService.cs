public class PosService
{
    private readonly DataContext _db;
    private readonly FiscalRegisterService _fiscalService;
    private readonly EGAISService _egaisService;
    private readonly PaymentTerminalService _paymentService;
    
    private Receipt _currentReceipt;

    public PosService(
        DataContext db, 
        FiscalRegisterService fiscalService,
        EGAISService egaisService,
        PaymentTerminalService paymentService)
    {
        _db = db;
        _fiscalService = fiscalService;
        _egaisService = egaisService;
        _paymentService = paymentService;
    }

    public void StartNewReceipt()
    {
        _currentReceipt = new Receipt
        {
            CreatedAt = DateTime.Now,
            Items = new List<ReceiptItem>()
        };
    }

    public void AddProductToReceipt(Product product, int quantity = 1)
    {
        var existingItem = _currentReceipt.Items.FirstOrDefault(i => i.Product.Id == product.Id);
        
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            _currentReceipt.Items.Add(new ReceiptItem
            {
                Product = product,
                Quantity = quantity,
                Price = product.Price
            });
        }
        
        _currentReceipt.TotalAmount = _currentReceipt.Items.Sum(i => i.Price * i.Quantity);
    }

    public async Task<Receipt> CompleteReceiptAsync(PaymentType paymentType)
    {
        _currentReceipt.PaymentType = paymentType;
        
        // Фискализация чека
        _currentReceipt.FiscalNumber = await _fiscalService.RegisterReceiptAsync(_currentReceipt);
        
        // Для алкоголя отправляем в ЕГАИС
        if (_currentReceipt.Items.Any(i => i.Product.IsAlcoholic))
        {
            _currentReceipt.EGAISCheckNumber = await _egaisService.RegisterSaleAsync(_currentReceipt);
        }
        
        // Обработка платежа
        bool paymentSuccess = await _paymentService.ProcessPaymentAsync(
            _currentReceipt.TotalAmount, 
            paymentType);
        
        if (!paymentSuccess)
        {
            throw new Exception("Payment processing failed");
        }
        
        // Сохраняем чек в БД
        _db.Receipts.Add(_currentReceipt);
        await _db.SaveChangesAsync();
        
        return _currentReceipt;
    }
}