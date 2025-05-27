using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BeerShopPOS.Models;
using BeerShopPOS.DAL;

namespace BeerShopPOS.Services
{
    public class PosService : BaseService, IPosService
    {
        private readonly DataContext _db;
        private readonly IProductService _productService;
        private readonly IFiscalRegisterService _fiscalService;
        private readonly IEGAISService _egaisService;
        private readonly IPaymentTerminalService _paymentService;

        public PosService(
            DataContext db,
            IProductService productService,
            IFiscalRegisterService fiscalService,
            IEGAISService egaisService,
            IPaymentTerminalService paymentService,
            ILogger<PosService> logger) : base(logger)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _fiscalService = fiscalService ?? throw new ArgumentNullException(nameof(fiscalService));
            _egaisService = egaisService ?? throw new ArgumentNullException(nameof(egaisService));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        public async Task<Receipt> CreateReceiptAsync()
        {
            return await ExecuteWithLoggingAsync(async () =>
            {
                var receipt = new Receipt
                {
                    Created = DateTime.Now,
                    Status = ReceiptStatus.Draft,
                    PaymentType = PaymentType.Cash,
                    Total = 0
                };

                _db.Receipts.Add(receipt);
                await _db.SaveChangesAsync();

                LogInfo($"Создан новый чек №{receipt.Id}");
                return receipt;
            }, "Создание нового чека");
        }

        public async Task<Receipt> AddProductToReceiptAsync(Receipt receipt, Product product)
        {
            return await ExecuteWithLoggingAsync(async () =>
            {
                if (product.RequiresEGAIS && !await _egaisService.ValidateProductAsync(product))
                {
                    throw new InvalidOperationException($"Товар {product.Name} не прошел проверку ЕГАИС");
                }

                // Load the receipt with its items from the database
                receipt = await _db.Receipts
                    .Include(r => r.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(r => r.Id == receipt.Id) 
                    ?? throw new InvalidOperationException("Receipt not found");

                var existingItem = receipt.Items.FirstOrDefault(i => i.ProductId == product.Id);
                if (existingItem != null)
                {
                    existingItem.Quantity += 1;
                }
                else
                {
                    receipt.Items.Add(new ReceiptItem
                    {
                        Product = product,
                        ProductId = product.Id,
                        Quantity = 1,
                        Price = product.Price,
                        Receipt = receipt,
                        ReceiptId = receipt.Id
                    });
                }

                // Update the stored total
                receipt.Total = receipt.ComputedTotal;
                
                // Save changes
                await _db.SaveChangesAsync();

                LogInfo($"Добавлен товар {product.Name} в чек №{receipt.Id}");
                return receipt;
            }, $"Добавление товара в чек {receipt.Id}");
        }

        public async Task<bool> FinalizeReceiptAsync(Receipt receipt, decimal amountPaid)
        {
            return await ExecuteWithLoggingAsync(async () =>
            {
                // Verify the total matches computed total
                if (receipt.Total != receipt.ComputedTotal)
                {
                    throw new InvalidOperationException("Ошибка в расчете суммы чека");
                }

                // Process payment
                if (!await _paymentService.ProcessPaymentAsync(receipt.Total))
                {
                    throw new InvalidOperationException("Ошибка проведения оплаты");
                }

                // Generate fiscal number only when finalizing receipt
                receipt.FiscalNumber = await _fiscalService.PrintReceiptAsync(receipt);

                // Register in EGAIS if needed
                if (receipt.Items.Any(i => i.Product.RequiresEGAIS))
                {
                    receipt.EGAISCheckNumber = await _egaisService.RegisterSaleAsync(receipt);
                }

                receipt.Status = ReceiptStatus.Completed;
                receipt.AmountPaid = amountPaid;
                receipt.IsPrinted = true;

                await _db.SaveChangesAsync();
                LogInfo($"Чек №{receipt.FiscalNumber} успешно закрыт");
                
                return true;
            }, $"Закрытие чека {receipt.Id}");
        }

        public async Task<bool> VoidReceiptAsync(Receipt receipt, string reason)
        {
            return await ExecuteWithLoggingAsync(async () =>
            {
                // Void fiscal receipt
                await _fiscalService.VoidReceiptAsync(receipt, reason);

                // Cancel EGAIS registration if needed
                if (!string.IsNullOrEmpty(receipt.EGAISCheckNumber))
                {
                    await _egaisService.CancelSaleAsync(receipt.EGAISCheckNumber);
                }

                // Void payment
                if (receipt.AmountPaid > 0)
                {
                    await _paymentService.VoidPaymentAsync(receipt.AmountPaid, receipt.FiscalNumber);
                }

                receipt.Status = ReceiptStatus.Voided;
                receipt.VoidReason = reason;
                
                await _db.SaveChangesAsync();
                LogInfo($"Чек №{receipt.Id} аннулирован");
                
                return true;
            }, $"Аннулирование чека {receipt.Id}");
        }

        public async Task<Receipt> UpdateReceiptAsync(Receipt receipt)
        {
            return await ExecuteWithLoggingAsync(async () =>
            {
                _db.Receipts.Update(receipt);
                await _db.SaveChangesAsync();
                return receipt;
            }, $"Обновление чека {receipt.Id}");
        }
    }
}