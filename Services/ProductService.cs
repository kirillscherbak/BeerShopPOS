public class ProductService
{
    private readonly DataContext _db;
    private readonly BarcodeScannerService _barcodeService;
    
    public ProductService(DataContext db, BarcodeScannerService barcodeService)
    {
        _db = db;
        _barcodeService = barcodeService;
    }

    public Product GetProductByBarcode(string barcode)
    {
        return _db.Products.FirstOrDefault(p => p.Barcode == barcode);
    }

    public async Task AddProductAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateStockAsync(int productId, int quantityChange)
    {
        var product = await _db.Products.FindAsync(productId);
        if (product != null)
        {
            product.StockQuantity += quantityChange;
            await _db.SaveChangesAsync();
        }
    }
}