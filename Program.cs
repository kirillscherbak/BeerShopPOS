public static class Program
{
    [STAThread]
    public static void Main()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
        
        var configuration = builder.Build();
        
        // Настройка DI-контейнера
        var services = new ServiceCollection();
        
        services.AddDbContext<DataContext>();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<ProductService>();
        services.AddSingleton<BarcodeScannerService>();
        services.AddSingleton<FiscalRegisterService>();
        services.AddSingleton<EGAISService>();
        services.AddSingleton<PaymentTerminalService>();
        services.AddSingleton<PosService>();
        services.AddSingleton<MainForm>();
        
        var serviceProvider = services.BuildServiceProvider();
        
        // Инициализация базы данных
        using (var scope = serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            db.Database.EnsureCreated();
        }
        
        // Запуск главной формы
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(serviceProvider.GetRequiredService<MainForm>());
    }
}