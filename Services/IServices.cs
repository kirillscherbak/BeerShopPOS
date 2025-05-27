using BeerShopPOS.Models;

namespace BeerShopPOS.Services
{
    public interface IProductService
    {
        Task<Product?> GetProductByBarcode(string barcode);
        Task<List<Product>> GetAllProducts();
        Task<Product> AddProduct(Product product);
        Task<Product> UpdateProduct(Product product);
        Task DeleteProduct(int id);
        Task UpdateStockAsync(int productId, int quantityChange);
    }

    public interface IBarcodeScannerService
    {
        event AsyncEventHandler<string>? BarcodeScanned;
        Task StartScanningAsync();
        Task StopScanningAsync();
        string? GetLastScannedBarcode();
    }

    public interface IFiscalRegisterService
    {
        Task OpenShiftAsync(string cashierName);
        Task CloseShiftAsync();
        Task<string> PrintReceiptAsync(Receipt receipt);
        Task VoidReceiptAsync(Receipt receipt, string reason);
    }

    public interface IEGAISService
    {
        Task<bool> ValidateProductAsync(Product product);
        Task<string> RegisterSaleAsync(Receipt receipt);
        Task<bool> CancelSaleAsync(string egaisCheckNumber);
    }

    public interface IPaymentTerminalService
    {
        Task<bool> ProcessPaymentAsync(decimal amount);
        Task<bool> VoidPaymentAsync(decimal amount, string transactionId);
    }

    public interface IPosService
    {
        Task<Receipt> CreateReceiptAsync();
        Task<Receipt> AddProductToReceiptAsync(Receipt receipt, Product product);
        Task<Receipt> UpdateReceiptAsync(Receipt receipt);
        Task<bool> FinalizeReceiptAsync(Receipt receipt, decimal amountPaid);
        Task<bool> VoidReceiptAsync(Receipt receipt, string reason);
    }
}
