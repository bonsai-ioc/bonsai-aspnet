namespace Bonsai.Extensions.DependencyInjection
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    class BonsaiServiceProvider : IServiceProvider, ISupportRequiredService, IDisposable
    {
        private readonly IScope _scope;

        public BonsaiServiceProvider(IScope scope)
        {
            _scope = scope;
        }

        public object GetService(Type serviceType)
        {
            return _scope.Resolve(serviceType);
        }

        public object GetRequiredService(Type serviceType)
        {
            return _scope.Resolve(serviceType) ?? throw new NotSupportedException(serviceType.ToString());
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}