namespace Bonsai.Extensions.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;

    public class BonsaiServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IScope _scope;

        public BonsaiServiceScopeFactory(IScope scope)
        {
            _scope = scope;
        }

        public IServiceScope CreateScope()
        {
            return new BonsaiServiceScope(_scope.CreateScope());
        }
    }
}