using Microsoft.Extensions.Logging;

namespace BeerShopPOS.UI
{
    public class FormLoggerFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public FormLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ILogger<T> CreateLogger<T>() where T : Form
        {
            return _loggerFactory.CreateLogger<T>();
        }
    }
}
