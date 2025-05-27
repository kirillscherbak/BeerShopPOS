#nullable enable

using System.Windows.Forms;
using System.Drawing;

namespace BeerShopPOS.UI
{
    partial class ProductForm
    {
        private System.ComponentModel.IContainer? components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            nameTextBox = new TextBox();
            barcodeTextBox = new TextBox();
            priceNumeric = new NumericUpDown();
            typeComboBox = new ComboBox();
            isAlcoholicCheck = new CheckBox();
            alcoholVolumeNumeric = new NumericUpDown();
            stockQuantityNumeric = new NumericUpDown();
            egaisCodeTextBox = new TextBox();
            egaisVolumeTextBox = new TextBox();
            descriptionTextBox = new TextBox();
            minStockNumeric = new NumericUpDown();
            isActiveCheck = new CheckBox();
            saveButton = new Button();
            cancelButton = new Button();

            tableLayoutPanel = new TableLayoutPanel();

            ((System.ComponentModel.ISupportInitialize)priceNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)alcoholVolumeNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)stockQuantityNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)minStockNumeric).BeginInit();
            SuspendLayout();

            // Form setup
            Text = "Добавить товар";
            MinimumSize = new Size(400, 600);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            // Table layout
            tableLayoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 13,
                Padding = new Padding(10),
                ColumnStyles = {
                    new ColumnStyle(SizeType.Percent, 40),
                    new ColumnStyle(SizeType.Percent, 60)
                }
            };

            // Add controls with labels
            AddControlWithLabel("Наименование:", nameTextBox);
            AddControlWithLabel("Штрихкод:", barcodeTextBox);
            AddControlWithLabel("Цена:", priceNumeric);
            AddControlWithLabel("Тип:", typeComboBox);
            AddControlWithLabel("Алкогольный:", isAlcoholicCheck);
            AddControlWithLabel("Крепость (%):", alcoholVolumeNumeric);
            AddControlWithLabel("Количество:", stockQuantityNumeric);
            AddControlWithLabel("Код ЕГАИС:", egaisCodeTextBox);
            AddControlWithLabel("Объём ЕГАИС:", egaisVolumeTextBox);
            AddControlWithLabel("Описание:", descriptionTextBox);
            AddControlWithLabel("Мин. остаток:", minStockNumeric);
            AddControlWithLabel("Активный:", isActiveCheck);

            // Setup numeric controls
            priceNumeric.DecimalPlaces = 2;
            priceNumeric.Maximum = 1000000;
            alcoholVolumeNumeric.DecimalPlaces = 1;
            alcoholVolumeNumeric.Maximum = 100;
            stockQuantityNumeric.Maximum = 1000000;
            minStockNumeric.Maximum = 1000000;

            // Buttons panel
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 40,
                Padding = new Padding(0, 0, 10, 0)
            };

            saveButton.Text = "Добавить";
            cancelButton.Text = "Отмена";
            saveButton.Click += SaveButton_Click;
            cancelButton.Click += CancelButton_Click;
            isAlcoholicCheck.CheckedChanged += IsAlcoholicCheck_CheckedChanged;

            buttonsPanel.Controls.Add(cancelButton);
            buttonsPanel.Controls.Add(saveButton);

            Controls.Add(tableLayoutPanel);
            Controls.Add(buttonsPanel);

            ((System.ComponentModel.ISupportInitialize)priceNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)alcoholVolumeNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)stockQuantityNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)minStockNumeric).EndInit();
            ResumeLayout(false);
        }

        private void AddControlWithLabel(string labelText, Control control)
        {
            var label = new Label
            {
                Text = labelText,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };

            control.Dock = DockStyle.Fill;
            control.Margin = new Padding(0, 2, 0, 2);

            tableLayoutPanel.Controls.Add(label);
            tableLayoutPanel.Controls.Add(control);
        }

        private TableLayoutPanel tableLayoutPanel = null!;
        private TextBox nameTextBox = null!;
        private TextBox barcodeTextBox = null!;
        private NumericUpDown priceNumeric = null!;
        private ComboBox typeComboBox = null!;
        private CheckBox isAlcoholicCheck = null!;
        private NumericUpDown alcoholVolumeNumeric = null!;
        private NumericUpDown stockQuantityNumeric = null!;
        private TextBox egaisCodeTextBox = null!;
        private TextBox egaisVolumeTextBox = null!;
        private TextBox descriptionTextBox = null!;
        private NumericUpDown minStockNumeric = null!;
        private CheckBox isActiveCheck = null!;
        private Button saveButton = null!;
        private Button cancelButton = null!;

        #endregion
    }
}
