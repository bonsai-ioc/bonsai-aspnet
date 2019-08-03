using System;

namespace Bonsai.Extensions.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        [Obsolete("please use UseBonsai instead")]
        public static IServiceCollection AddBonsai(this IServiceCollection services,
            Action<ContainerBuilder> configurationAction = null)
        {
            return UseBonsai(services, configurationAction);
        }

        /// <summary>
        /// this is the ASPNET Core 2.x method to use Bonsai as the IoC container.
        /// </summary>
        public static IServiceCollection UseBonsai(this IServiceCollection services,
            Action<ContainerBuilder> configurationAction = null)
        {
            return services.AddSingleton<IServiceProviderFactory<ContainerBuilder>>(
                new BonsaiServiceProviderFactory(configurationAction));
        }
    }
}