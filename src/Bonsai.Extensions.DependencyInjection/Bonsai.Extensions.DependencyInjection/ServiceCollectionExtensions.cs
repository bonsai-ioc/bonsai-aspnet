using System;

namespace Bonsai.Extensions.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBonsai(this IServiceCollection services,
            Action<ContainerBuilder> configurationAction = null)
        {
            return services.AddSingleton<IServiceProviderFactory<ContainerBuilder>>(
                new BonsaiServiceProviderFactory(configurationAction));
        }
    }
}