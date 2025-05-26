public class FiscalRegisterService
{
    private readonly IConfiguration _config;
    
    public FiscalRegisterService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<string> RegisterReceiptAsync(Receipt receipt)
    {
        // Здесь используется библиотека производителя ФН
        // Пример для Атол:
        var driver = new AtolDriver(_config["Fiscal:ComPort"]);
        
        try
        {
            driver.Open();
            
            // Открытие чека
            driver.OpenReceipt(receipt.Id.ToString());
            
            // Добавление позиций
            foreach (var item in receipt.Items)
            {
                driver.AddPosition(
                    item.Product.Name,
                    item.Price,
                    item.Quantity,
                    item.Product.IsAlcoholic ? "ALCOHOL" : "COMMODITY");
            }
            
            // Регистрация оплаты
            driver.AddPayment(receipt.TotalAmount, 
                receipt.PaymentType == PaymentType.Cash ? "CASH" : "CARD");
            
            // Закрытие чека
            var fiscalData = driver.CloseReceipt();
            
            return fiscalData.FiscalNumber;
        }
        finally
        {
            driver.Close();
        }
    }
}