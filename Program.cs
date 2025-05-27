using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using BeerShopPOS.Models;
using BeerShopPOS.Services;
using BeerShopPOS.UI;
using BeerShopPOS.DAL;

namespace BeerShopPOS
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            
            var configuration = builder.Build();

            // Configure logging
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.File("logs/beershop.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            
            try
            {
                var services = ConfigureServices(configuration);
                using var serviceProvider = services.BuildServiceProvider();
                
                // Initialize database
                using (var scope = serviceProvider.CreateScope())
                {
                    try
                    {
                        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                        db.Database.Migrate();
                        Log.Information("Database initialized successfully");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error initializing database");
                        MessageBox.Show($"Database initialization error: {ex.Message}", 
                            "Error", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Error);
                        return;
                    }
                }
                
                // Run main form
                using var mainScope = serviceProvider.CreateScope();
                var mainForm = mainScope.ServiceProvider.GetRequiredService<MainForm>();
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
                MessageBox.Show($"Critical error: {ex.Message}", 
                    "Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IServiceCollection ConfigureServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            // Add logging
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(dispose: true);
            });

            // Add Configuration
            services.AddSingleton(configuration);

            // Add Database Context
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("Default"));
            });

            // Add FormLoggerFactory
            services.AddSingleton<FormLoggerFactory>();

            // Add Services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBarcodeScannerService, BarcodeScannerService>();
            services.AddScoped<IFiscalRegisterService, FiscalRegisterService>();
            services.AddScoped<IEGAISService, EGAISService>();
            services.AddScoped<IPaymentTerminalService, PaymentTerminalService>();
            services.AddScoped<IPosService, PosService>();

            // Add Forms
            services.AddTransient<MainForm>();

            return services;
        }
    }
}