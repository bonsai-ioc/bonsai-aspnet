using System;

namespace Bonsai.Extensions.DependencyInjection
{
    using System.Collections.Generic;
    using System.Reflection;
    using LifeStyles;
    using Microsoft.Extensions.DependencyInjection;
    using Registry;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBonsai(this IServiceCollection services,
            Action<ContainerBuilder> configurationAction = null)
        {
            return services.AddSingleton<IServiceProviderFactory<ContainerBuilder>>(
                new BonsaiServiceProviderFactory(configurationAction));
        }
    }

    class BonsaiServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private Action<ContainerBuilder> _configurationAction;

        public BonsaiServiceProviderFactory(Action<ContainerBuilder> configurationAction = null)
        {
            _configurationAction = configurationAction ?? (builder => { });
        }


        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.Populate(services);
            _configurationAction(builder);
            return builder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            var container = containerBuilder.Create();
            return new BonsaiServiceProvider((IAdvancedScope) container);
        }
    }


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

    public static class BonsaiRegistration
    {
        public static void Populate(
            this ContainerBuilder builder,
            IEnumerable<ServiceDescriptor> descriptors)
        {
            builder.Register<BonsaiServiceProvider>().As<IServiceProvider>().Scoped<PerScope>();
            builder.Register<BonsaiServiceScopeFactory>().As<IServiceScopeFactory>().Scoped<PerScope>();
            Register(builder, descriptors);
        }


        private static FluentBuilder Scope(
            this FluentBuilder builder,
            ServiceLifetime lifecycleKind
        )
        {
            switch (lifecycleKind)
            {
                case ServiceLifetime.Singleton:
                    builder.Scoped<Singleton>();
                    break;
                case ServiceLifetime.Scoped:
                    builder.Scoped<PerScope>();
                    break;
                case ServiceLifetime.Transient:
                    builder.Scoped<Transient>();
                    break;
            }

            return builder;
        }

        private static void Register(
            ContainerBuilder builder,
            IEnumerable<ServiceDescriptor> descriptors)
        {
            foreach (var descriptor in descriptors)
            {
                if (descriptor.ImplementationType != null)
                {
                    // Test if the an open generic type is being registered
                    var serviceTypeInfo = descriptor.ServiceType.GetTypeInfo();
                    if (serviceTypeInfo.IsGenericTypeDefinition)
                    {
                        builder
                            .Register(descriptor.ImplementationType)
                            .As(descriptor.ServiceType)
                            .Scope(descriptor.Lifetime);
                    }
                    else
                    {
                        builder
                            .Register(descriptor.ImplementationType)
                            .As(descriptor.ServiceType)
                            .Scope(descriptor.Lifetime);
                    }
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    builder.Register((scope) =>
                            descriptor.ImplementationFactory(scope.Resolve<IServiceProvider>("default"))
                        )
                        .Scope(descriptor.Lifetime);
                }
                else
                {
                    builder
                        .Register(descriptor.ServiceType)
                        .UsingInstance(descriptor.ImplementationInstance)
                        .As(descriptor.ServiceType)
                        .Scope(descriptor.Lifetime);
                }
            }
        }
    }
}