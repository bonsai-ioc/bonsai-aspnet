using System;
namespace Bonsai.Extensions.Hosting
{
    using DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class HostBuilderExtensions
    {
        /// <summary>
        /// this is mainly for 3.x support, using this method will make Bonsai your main IoC container.
        /// </summary>
        public static IHostBuilder UseBonsai(this IHostBuilder builder, Action<ContainerBuilder> configurationAction = null)
        {
            return builder.UseServiceProviderFactory(new BonsaiServiceProviderFactory(configurationAction));
        }
    }
}
