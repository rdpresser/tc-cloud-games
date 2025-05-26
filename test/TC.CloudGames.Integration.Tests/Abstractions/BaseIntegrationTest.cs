using Bogus;

namespace TC.CloudGames.Integration.Tests.Abstractions
{
    [Collection(nameof(IntegrationTestCollection))]
    public class BaseIntegrationTest : IDisposable
    {
        private bool _disposed;
        protected static Faker Faker => new();
        private readonly IServiceScope _scope;

        public BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            _scope = factory.Services.CreateScope();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _scope?.Dispose();
                }

                // Dispose unmanaged resources if any
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~BaseIntegrationTest()
        {
            Dispose(disposing: false);
        }
    }
}
