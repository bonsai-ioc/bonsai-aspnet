namespace Bonsai.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    class BonsaiServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private Action<ContainerBuilder> _configurationAction;

        public BonsaiServiceProviderFactory(Action<ContainerBuilder> configurationAction = null)
        {
            _configurationAction = configurationAction ?? (builder => { });
        }


        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var test = Where(services, "ModelBin");
            
            var builder = new ContainerBuilder();
            builder.Populate(services);
            _configurationAction(builder);
            return builder;
        }

        private static IEnumerable<ServiceDescriptor> Where(IServiceCollection collection,string name)
        {
            return collection.Where(x => x.ServiceType.Name.ToLower().Contains(name.ToLower())).ToList();
        }
        

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            var container = containerBuilder.Create();
            return new BonsaiServiceProvider((IAdvancedScope) container);
        }
    }
}