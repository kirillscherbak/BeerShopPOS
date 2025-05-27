using System;
using System.Windows.Forms;
using BeerShopPOS.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace BeerShopPOS.UI
{
    public partial class ProductForm : Form
    {
        private readonly ILogger<ProductForm> _logger;
        private Product? _product;

        public Product? Result => _product;

        public ProductForm(ILogger<ProductForm> logger, Product? existingProduct = null)
        {
            _logger = logger;
            InitializeComponent();
            SetupComboBox();

            if (existingProduct != null)
            {
                _product = existingProduct;
                LoadProduct();
            }
        }

        private void SetupComboBox()
        {
            typeComboBox.DataSource = Enum.GetValues(typeof(ProductType));
            typeComboBox.SelectedItem = ProductType.Beer;
        }

        private void LoadProduct()
        {
            if (_product == null) return;

            nameTextBox.Text = _product.Name;
            barcodeTextBox.Text = _product.Barcode;
            priceNumeric.Value = _product.Price;
            typeComboBox.SelectedItem = _product.Type;
            isAlcoholicCheck.Checked = _product.IsAlcoholic;
            alcoholVolumeNumeric.Value = (decimal)_product.AlcoholVolume;
            stockQuantityNumeric.Value = _product.StockQuantity;
            egaisCodeTextBox.Text = _product.EGAISCode;
            egaisVolumeTextBox.Text = _product.EGAISVolume;
            descriptionTextBox.Text = _product.Description;
            minStockNumeric.Value = _product.MinStockQuantity;
            isActiveCheck.Checked = _product.IsActive;

            Text = "Редактировать товар";
            saveButton.Text = "Сохранить";
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                var product = _product ?? new Product();

                product.Name = nameTextBox.Text;
                product.Barcode = barcodeTextBox.Text;
                product.Price = priceNumeric.Value;
                product.Type = (ProductType)typeComboBox.SelectedItem;
                product.IsAlcoholic = isAlcoholicCheck.Checked;
                product.AlcoholVolume = (float)alcoholVolumeNumeric.Value;
                product.StockQuantity = (int)stockQuantityNumeric.Value;
                product.EGAISCode = egaisCodeTextBox.Text;
                product.EGAISVolume = egaisVolumeTextBox.Text;
                product.Description = descriptionTextBox.Text;
                product.MinStockQuantity = (int)minStockNumeric.Value;
                product.IsActive = isActiveCheck.Checked;

                if (_product == null)
                {
                    product.CreatedAt = DateTime.Now;
                }
                else
                {
                    product.LastModifiedAt = DateTime.Now;
                }

                var context = new ValidationContext(product);
                Validator.ValidateObject(product, context, true);

                _product = product;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving product");
                MessageBox.Show("Ошибка при сохранении товара", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void IsAlcoholicCheck_CheckedChanged(object sender, EventArgs e)
        {
            alcoholVolumeNumeric.Enabled = isAlcoholicCheck.Checked;
            egaisCodeTextBox.Enabled = isAlcoholicCheck.Checked;
            egaisVolumeTextBox.Enabled = isAlcoholicCheck.Checked;
        }
    }
}
