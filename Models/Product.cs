using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeerShopPOS.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Barcode { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public ProductType Type { get; set; }

        [Required]
        public bool IsAlcoholic { get; set; }

        [Range(0, 100)]
        public float AlcoholVolume { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        // EGAIS specific properties
        [MaxLength(50)]
        public string? EGAISCode { get; set; }

        [MaxLength(20)]
        public string? EGAISVolume { get; set; }

        // Additional properties
        [Required]
        public bool IsActive { get; set; } = true;

        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int MinStockQuantity { get; set; } = 10;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? LastModifiedAt { get; set; }

        [NotMapped]
        public bool RequiresEGAIS => IsAlcoholic && !string.IsNullOrEmpty(EGAISCode);
    }

    public enum ProductType
    {
        Beer,
        Wine,
        Spirits,
        SoftDrink,
        Snack,
        Tobacco,
        Other
    }
}