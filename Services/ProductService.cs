using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using BeerShopPOS.Models;
using BeerShopPOS.DAL;

namespace BeerShopPOS.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly DataContext _db;

        public ProductService(DataContext db, ILogger<ProductService> logger)
            : base(logger)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<Product?> GetProductByBarcode(string barcode)
        {
            return await ExecuteWithLoggingAsync(async () =>
            {
                var product = await _db.Products.FirstOrDefaultAsync(p => p.Barcode == barcode);
                if (product == null)
                {
                    LogWarning($"Товар не найден по штрихкоду: {barcode}");
                }
                return product;
            }, $"Поиск товара по штрихкоду {barcode}");
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await ExecuteWithLoggingAsync(async () =>
            {
                return await _db.Products.ToListAsync();
            }, "Получение списка всех товаров");
        }

        public async Task<Product> AddProduct(Product product)
        {
            await ExecuteWithLoggingAsync(async () =>
            {
                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                LogInfo($"Товар {product.Name} успешно добавлен");
                return true;
            }, $"Добавление товара {product.Name}");
            return product;
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            await ExecuteWithLoggingAsync(async () =>
            {
                _db.Products.Update(product);
                await _db.SaveChangesAsync();
                LogInfo($"Товар {product.Name} успешно обновлен");
                return true;
            }, $"Обновление товара {product.Name}");
            return product;
        }

        public async Task DeleteProduct(int id)
        {
            await ExecuteWithLoggingAsync(async () =>
            {
                var product = await _db.Products.FindAsync(id)
                    ?? throw new InvalidOperationException($"Товар {id} не найден");
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
                LogInfo($"Товар {product.Name} успешно удален");
                return true;
            }, $"Удаление товара {id}");
        }

        public async Task UpdateStockAsync(int productId, int quantityChange)
        {
            await ExecuteWithLoggingAsync(async () =>
            {
                var product = await _db.Products.FindAsync(productId)
                    ?? throw new InvalidOperationException($"Товар {productId} не найден");

                product.StockQuantity += quantityChange;
                
                if (product.StockQuantity < 0)
                {
                    throw new InvalidOperationException($"Недостаточно товара {product.Name} на складе");
                }

                await _db.SaveChangesAsync();
                LogInfo($"Количество товара {product.Name} изменено на {quantityChange}");
                return true;
            }, $"Обновление остатков товара {productId}");
        }
    }
}