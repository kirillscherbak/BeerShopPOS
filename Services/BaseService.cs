using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BeerShopPOS.Services
{
    public abstract class BaseService : IDisposable
    {
        protected readonly ILogger _logger;
        private bool _disposed;

        protected BaseService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected virtual async Task<T> ExecuteWithLoggingAsync<T>(Func<Task<T>> action, string operationName)
        {
            try
            {
                _logger.LogInformation($"Starting {operationName}");
                var result = await action();
                _logger.LogInformation($"Completed {operationName} successfully");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during {operationName}: {ex.Message}");
                throw;
            }
        }

        protected virtual T ExecuteWithLogging<T>(Func<T> action, string operationName)
        {
            try
            {
                _logger.LogInformation($"Starting {operationName}");
                var result = action();
                _logger.LogInformation($"Completed {operationName} successfully");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during {operationName}: {ex.Message}");
                throw;
            }
        }

        protected virtual void LogError(Exception ex, string message)
        {
            _logger.LogError(ex, message);
        }

        protected virtual void LogInfo(string message)
        {
            _logger.LogInformation(message);
        }

        protected virtual void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DisposeManagedResources();
                }

                DisposeUnmanagedResources();
                _disposed = true;
            }
        }

        protected virtual void DisposeManagedResources()
        {
            // Override in derived classes to dispose managed resources
        }

        protected virtual void DisposeUnmanagedResources()
        {
            // Override in derived classes to dispose unmanaged resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BaseService()
        {
            Dispose(false);
        }
    }
}
