public class DataContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Receipt> Receipts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=beershop.db");
    }
}