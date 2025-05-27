using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BeerShopPOS.Models
{
    public enum PaymentType
    {
        Cash,
        Card,
        SBP,
        Other
    }

    public enum ReceiptStatus
    {
        Draft,
        Completed,
        Voided
    }

    public class Receipt
    {
        public int Id { get; set; }
        
        [Required]
        public DateTime Created { get; set; }
        
        [Required]
        public List<ReceiptItem> Items { get; set; } = new();
        
        [Required]
        public PaymentType PaymentType { get; set; } = PaymentType.Cash;  // Default to Cash
        
        [Required]
        public ReceiptStatus Status { get; set; }
        
        public string? FiscalNumber { get; set; }
        public string? EGAISCheckNumber { get; set; }
        public string? VoidReason { get; set; }
        public decimal AmountPaid { get; set; }
        public bool IsPrinted { get; set; }
        public string? CashierName { get; set; }

        // Computed property for easy access to total
        public decimal ComputedTotal => Items.Sum(item => item.Price * item.Quantity);
        
        // Property for database storage
        [Required]
        public decimal Total { get; set; }
    }

    public class ReceiptItem
    {
        public int Id { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public Product Product { get; set; } = null!;
        
        public int ReceiptId { get; set; }
        
        [Required]
        public Receipt Receipt { get; set; } = null!;
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        public int VoidedQuantity { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }
}