using System;
using System.Media;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using BeerShopPOS.Models;
using BeerShopPOS.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;

namespace BeerShopPOS.UI
{
    public partial class MainForm : Form
    {
        private readonly IPosService _posService;
        private readonly IBarcodeScannerService _barcodeScanner;
        private readonly IProductService _productService;
        private readonly ILogger<MainForm> _logger;
        private readonly FormLoggerFactory _formLoggerFactory;
        private Receipt? _currentReceipt;
        private List<Product> _searchResults = new();

        public MainForm(
            IPosService posService,
            IBarcodeScannerService barcodeScanner,
            IProductService productService,
            ILoggerFactory loggerFactory)
        {
            _posService = posService ?? throw new ArgumentNullException(nameof(posService));
            _barcodeScanner = barcodeScanner ?? throw new ArgumentNullException(nameof(barcodeScanner));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));            _logger = loggerFactory.CreateLogger<MainForm>();
            _formLoggerFactory = new FormLoggerFactory(loggerFactory);

            InitializeComponent();
            SetupEventHandlers();
            SetupProductsGrid();
            SetupReceiptItemsGrid();
            _ = StartNewReceipt();
            _ = LoadAllProducts();
        }

        private void SetupEventHandlers()
        {            _barcodeScanner.BarcodeScanned += OnBarcodeScanned;
            Load += async (s, e) => await _barcodeScanner.StartScanningAsync();
            FormClosing += async (s, e) => await _barcodeScanner.StopScanningAsync();
            searchBox.KeyPress += (s, e) => { if (e.KeyChar == (char)Keys.Enter) _ = SearchProducts(); };
        }

        private void SetupProductsGrid()
        {
            productsGrid.Columns.Clear();
            productsGrid.AutoGenerateColumns = false;

            productsGrid.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Name", 
                HeaderText = "Наименование", 
                DataPropertyName = "Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            
            productsGrid.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Price", 
                HeaderText = "Цена (₽)", 
                DataPropertyName = "Price",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
            
            productsGrid.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "StockQuantity", 
                HeaderText = "Остаток", 
                DataPropertyName = "StockQuantity",
                Width = 70,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
        }

        private async Task LoadAllProducts()
        {
            try
            {
                _searchResults = await _productService.GetAllProducts();
                UpdateProductsGrid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                MessageBox.Show("Ошибка загрузки товаров", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SearchProducts()
        {
            try
            {
                var searchText = searchBox.Text.Trim();
                if (string.IsNullOrEmpty(searchText))
                {
                    await LoadAllProducts();
                    return;
                }

                var allProducts = await _productService.GetAllProducts();
                _searchResults = allProducts.Where(p => 
                    p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    p.Barcode.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                UpdateProductsGrid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                MessageBox.Show("Ошибка поиска товаров", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateProductsGrid()
        {
            productsGrid.DataSource = null;
            productsGrid.DataSource = _searchResults;

            if (productsGrid.Columns["Price"] != null)
            {
                productsGrid.Columns["Price"].DefaultCellStyle.Format = "N2";
                productsGrid.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                productsGrid.Columns["Price"].HeaderText = "Цена (₽)";
            }
        }

        private async void SearchButton_Click(object sender, EventArgs e)
        {
            await SearchProducts();
        }

        private async void ProductsGrid_DoubleClick(object sender, EventArgs e)
        {
            if (productsGrid.CurrentRow?.DataBoundItem is Product product)
            {
                await AddProductToReceipt(product);
            }
        }

        private async Task OnBarcodeScanned(object? sender, string barcode)
        {
            try
            {
                if (_currentReceipt == null)
                {
                    await StartNewReceipt();
                }

                var product = await _productService.GetProductByBarcode(barcode);
                if (product != null)
                {
                    await AddProductToReceipt(product);
                    PlaySuccessSound();
                }
                else
                {
                    PlayErrorSound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing barcode");                // Use Invoke for UI operations from a background thread
                await Task.Run(() => this.Invoke((MethodInvoker)delegate 
                {
                    MessageBox.Show($"Ошибка при обработке штрих-кода: {ex.Message}",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    PlayErrorSound();
                }));
            }
        }        private async Task StartNewReceipt()
        {
            _currentReceipt = await _posService.CreateReceiptAsync();
            await UpdateUIAsync();
        }        private async Task AddProductToReceipt(Product product)
        {
            if (_currentReceipt == null) return;

            try
            {
                _currentReceipt = await _posService.AddProductToReceiptAsync(_currentReceipt, product);
                await UpdateUIAsync();
                PlaySuccessSound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product to receipt");
                MessageBox.Show($"Ошибка при добавлении товара: {ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                PlayErrorSound();
            }
        }        private async Task UpdateUIAsync()
        {
            if (!this.IsHandleCreated) return;
            
            await Task.Run(() => this.Invoke((MethodInvoker)delegate
            {
                if (_currentReceipt == null)
                {
                    receiptItemsGrid.DataSource = null;
                    totalLabel.Text = "0,00 ₽";
                    return;
                }

                var bindingList = new BindingList<ReceiptItem>(new List<ReceiptItem>(_currentReceipt.Items));
                receiptItemsGrid.DataSource = bindingList;
                totalLabel.Text = $"{_currentReceipt.ComputedTotal:N2} ₽";

                // Refresh grid to update totals
                receiptItemsGrid.Refresh();
            }));
        }

        private void SetupReceiptItemsGrid()
        {
            receiptItemsGrid.Columns.Clear();
            receiptItemsGrid.AutoGenerateColumns = false;
            
            receiptItemsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductName",
                HeaderText = "Наименование",
                DataPropertyName = "Product.Name",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            receiptItemsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Price",
                HeaderText = "Цена (₽)",
                DataPropertyName = "Price",
                ReadOnly = true,
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "N2", 
                    Alignment = DataGridViewContentAlignment.MiddleRight 
                }
            });

            receiptItemsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Quantity",
                HeaderText = "Кол-во",
                DataPropertyName = "Quantity",
                Width = 70,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight 
                }
            });

            receiptItemsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Total",
                HeaderText = "Сумма (₽)",
                ReadOnly = true,
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "N2", 
                    Alignment = DataGridViewContentAlignment.MiddleRight 
                }
            });

            receiptItemsGrid.CellValueNeeded += (s, e) =>
            {
                if (e.ColumnIndex == receiptItemsGrid.Columns["Total"].Index && e.RowIndex >= 0)
                {
                    var item = (ReceiptItem)receiptItemsGrid.Rows[e.RowIndex].DataBoundItem;
                    e.Value = item.Price * item.Quantity;
                }
            };

            receiptItemsGrid.CellValidating += (s, e) =>
            {
                if (e.ColumnIndex == receiptItemsGrid.Columns["Quantity"].Index)
                {
                    var value = e.FormattedValue?.ToString();
                    if (value == null || !int.TryParse(value, out int quantity) || quantity <= 0)
                    {
                        e.Cancel = true;
                        MessageBox.Show("Количество должно быть целым положительным числом",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }
            };

            receiptItemsGrid.CellEndEdit += async (s, e) =>
            {
                if (e.ColumnIndex == receiptItemsGrid.Columns["Quantity"].Index && _currentReceipt != null)
                {
                    try
                    {
                        var item = (ReceiptItem)receiptItemsGrid.Rows[e.RowIndex].DataBoundItem;
                        _currentReceipt.Total = _currentReceipt.ComputedTotal;
                        await _posService.UpdateReceiptAsync(_currentReceipt);
                        await UpdateUIAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating receipt item quantity");
                        MessageBox.Show($"Ошибка при изменении количества: {ex.Message}",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            };
        }

        private async void PayButton_Click(object sender, EventArgs e)
        {
            if (_currentReceipt == null) return;

            try
            {
                decimal amountPaid = _currentReceipt.Total;
                await _posService.FinalizeReceiptAsync(_currentReceipt, amountPaid);
                await StartNewReceipt();
                PlaySuccessSound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finalizing receipt");
                MessageBox.Show($"Ошибка при оплате чека: {ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                PlayErrorSound();
            }
        }

        private void PlaySuccessSound()
        {
            SystemSounds.Asterisk.Play();
        }

        private void PlayErrorSound()
        {
            SystemSounds.Exclamation.Play();
        }

        private async void AddProductButton_Click(object sender, EventArgs e)
        {            try
            {
                using var form = new ProductForm(_formLoggerFactory.CreateLogger<ProductForm>());
                if (form.ShowDialog() == DialogResult.OK && form.Result != null)
                {
                    await _productService.AddProduct(form.Result);
                    await LoadAllProducts();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product");
                MessageBox.Show("Ошибка при добавлении товара", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void EditProductButton_Click(object sender, EventArgs e)
        {
            if (productsGrid.CurrentRow?.DataBoundItem is not Product product)
            {
                MessageBox.Show("Выберите товар для редактирования", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {                using var form = new ProductForm(_formLoggerFactory.CreateLogger<ProductForm>(), product);
                if (form.ShowDialog() == DialogResult.OK && form.Result != null)
                {
                    await _productService.UpdateProduct(form.Result);
                    await LoadAllProducts();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                MessageBox.Show("Ошибка при обновлении товара", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DeleteProductButton_Click(object sender, EventArgs e)
        {
            if (productsGrid.CurrentRow?.DataBoundItem is not Product product)
            {
                MessageBox.Show("Выберите товар для удаления", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                if (MessageBox.Show(
                    $"Вы уверены, что хотите удалить товар '{product.Name}'?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    await _productService.DeleteProduct(product.Id);
                    await LoadAllProducts();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product");
                MessageBox.Show("Ошибка при удалении товара", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void RemoveItemButton_Click(object sender, EventArgs e)
        {
            if (_currentReceipt == null) return;

            if (receiptItemsGrid.CurrentRow?.DataBoundItem is not ReceiptItem item)
            {
                MessageBox.Show(
                    "Выберите позицию для удаления",
                    "Информация",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            try
            {
                _currentReceipt.Items.Remove(item);
                _currentReceipt.Total = _currentReceipt.ComputedTotal;
                await _posService.UpdateReceiptAsync(_currentReceipt);
                await UpdateUIAsync();
                PlaySuccessSound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from receipt");
                MessageBox.Show(
                    $"Ошибка при удалении позиции: {ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                PlayErrorSound();
            }
        }
    }
}