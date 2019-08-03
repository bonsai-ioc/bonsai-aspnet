namespace Bonsai.Extensions.DependencyInjection
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    class BonsaiServiceScope : IServiceScope
    {
        readonly BonsaiServiceProvider _provider;

        public BonsaiServiceScope(IScope scope)
        {
            _provider = new BonsaiServiceProvider(scope);
        }


        public void Dispose()
        {
            _provider.Dispose();
        }

        public IServiceProvider ServiceProvider => _provider;
    }
}