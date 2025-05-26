public class Receipt
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ReceiptItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public PaymentType PaymentType { get; set; }
    public string FiscalNumber { get; set; }
    public string EGAISCheckNumber { get; set; }
}

public class ReceiptItem
{
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public enum PaymentType
{
    Cash,
    Card,
    SBP,
    Other
}