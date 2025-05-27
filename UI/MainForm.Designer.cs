#nullable enable

using System.Windows.Forms;

namespace BeerShopPOS.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer? components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                
                if (_barcodeScanner != null)
                {
                    _barcodeScanner.BarcodeScanned -= OnBarcodeScanned;
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            
            // Receipt panel (left side)
            receiptPanel = new Panel();
            receiptItemsGrid = new DataGridView();
            totalLabel = new Label();
            paymentPanel = new Panel();
            cashRadio = new RadioButton();
            cardRadio = new RadioButton();
            payButton = new Button();
            removeItemButton = new Button();

            // Catalog panel (right side)
            catalogPanel = new Panel();
            searchPanel = new Panel();
            searchBox = new TextBox();
            searchButton = new Button();
            addProductButton = new Button();
            editProductButton = new Button();
            deleteProductButton = new Button();
            productsGrid = new DataGridView();

            ((System.ComponentModel.ISupportInitialize)receiptItemsGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)productsGrid).BeginInit();
            receiptPanel.SuspendLayout();
            paymentPanel.SuspendLayout();
            catalogPanel.SuspendLayout();
            searchPanel.SuspendLayout();
            SuspendLayout();

            // Receipt panel setup
            receiptPanel.Dock = DockStyle.Left;
            receiptPanel.Width = 400;
            receiptPanel.Controls.Add(receiptItemsGrid);
            receiptPanel.Controls.Add(totalLabel);
            receiptPanel.Controls.Add(paymentPanel);
            receiptPanel.Controls.Add(payButton);
            receiptPanel.Controls.Add(removeItemButton);

            // Receipt items grid
            receiptItemsGrid.Dock = DockStyle.Top;
            receiptItemsGrid.Height = 400;
            receiptItemsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            receiptItemsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            receiptItemsGrid.AllowUserToAddRows = false;

            // Remove item button
            removeItemButton.Text = "Удалить";
            removeItemButton.Dock = DockStyle.Bottom;
            removeItemButton.Height = 30;
            removeItemButton.Click += RemoveItemButton_Click;
            receiptPanel.Controls.Add(removeItemButton);

            // Payment panel
            paymentPanel.Dock = DockStyle.Bottom;
            paymentPanel.Height = 50;
            paymentPanel.Controls.Add(cashRadio);
            paymentPanel.Controls.Add(cardRadio);

            cashRadio.Text = "Наличные";
            cashRadio.Checked = true;
            cashRadio.Location = new System.Drawing.Point(10, 10);

            cardRadio.Text = "Карта";
            cardRadio.Location = new System.Drawing.Point(120, 10);

            // Pay button
            payButton.Text = "Оплатить";
            payButton.Dock = DockStyle.Bottom;
            payButton.Height = 40;
            payButton.Click += PayButton_Click;

            // Total label
            totalLabel.Text = "0.00";
            totalLabel.Dock = DockStyle.Bottom;
            totalLabel.Height = 30;
            totalLabel.TextAlign = ContentAlignment.MiddleRight;
            totalLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);

            // Catalog panel setup
            catalogPanel.Dock = DockStyle.Fill;
            catalogPanel.Controls.Add(productsGrid);
            catalogPanel.Controls.Add(searchPanel);

            // Search panel
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Height = 50;
            searchPanel.Controls.Add(deleteProductButton);
            searchPanel.Controls.Add(editProductButton);
            searchPanel.Controls.Add(addProductButton);
            searchPanel.Controls.Add(searchButton);
            searchPanel.Controls.Add(searchBox);

            searchBox.Location = new System.Drawing.Point(10, 10);
            searchBox.Width = 200;
            searchBox.PlaceholderText = "Поиск товара...";

            searchButton.Text = "Найти";
            searchButton.Location = new System.Drawing.Point(220, 10);
            searchButton.Width = 70;
            searchButton.Click += SearchButton_Click;

            addProductButton.Text = "Добавить";
            addProductButton.Location = new System.Drawing.Point(300, 10);
            addProductButton.Width = 80;
            addProductButton.Click += AddProductButton_Click;

            editProductButton.Text = "Изменить";
            editProductButton.Location = new System.Drawing.Point(390, 10);
            editProductButton.Width = 80;
            editProductButton.Click += EditProductButton_Click;

            deleteProductButton.Text = "Удалить";
            deleteProductButton.Location = new System.Drawing.Point(480, 10);
            deleteProductButton.Width = 80;
            deleteProductButton.Click += DeleteProductButton_Click;

            // Products grid
            productsGrid.Dock = DockStyle.Fill;
            productsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            productsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            productsGrid.AllowUserToAddRows = false;
            productsGrid.DoubleClick += ProductsGrid_DoubleClick;

            // Form setup
            MinimumSize = new System.Drawing.Size(900, 600);
            Controls.Add(catalogPanel);
            Controls.Add(receiptPanel);
            Text = "BeerShopPOS";

            ((System.ComponentModel.ISupportInitialize)receiptItemsGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)productsGrid).EndInit();
            receiptPanel.ResumeLayout(false);
            paymentPanel.ResumeLayout(false);
            catalogPanel.ResumeLayout(false);
            searchPanel.ResumeLayout(false);
            searchPanel.PerformLayout();
            ResumeLayout(false);
        }

        private Panel receiptPanel = null!;
        private DataGridView receiptItemsGrid = null!;
        private Label totalLabel = null!;
        private Panel paymentPanel = null!;
        private RadioButton cashRadio = null!;
        private RadioButton cardRadio = null!;
        private Button payButton = null!;
        private Button removeItemButton = null!;

        private Panel catalogPanel = null!;
        private Panel searchPanel = null!;
        private TextBox searchBox = null!;
        private Button searchButton = null!;
        private Button addProductButton = null!;
        private Button editProductButton = null!;
        private Button deleteProductButton = null!;
        private DataGridView productsGrid = null!;

        #endregion
    }
}