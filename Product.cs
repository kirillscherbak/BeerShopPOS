public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Barcode { get; set; }
    public decimal Price { get; set; }
    public ProductType Type { get; set; }
    public bool IsAlcoholic { get; set; }
    public float AlcoholVolume { get; set; }
    public int StockQuantity { get; set; }
    
    // Для алкогольной продукции
    public string EGAISCode { get; set; }
    public string EGAISVolume { get; set; }
}

public enum ProductType
{
    Beer,
    SoftDrink,
    Snack,
    Other
}