namespace Bonsai.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Bonsai.LifeStyles;
    using Microsoft.Extensions.DependencyInjection;
    using Bonsai.Registry;

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
            ServiceLifetime lifecycleKind)
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

            var items = descriptors.GroupBy(x => x.ServiceType).Select(grp => new
            {
                descriptors = grp,
                count = grp.Count()
            }).OrderByDescending(x => x.count).ToList();

            foreach (var descriptor in descriptors)
            {
                if (descriptor.ImplementationType != null)
                {

                    builder
                        .Register(descriptor.ImplementationType)
                        .As(descriptor.ServiceType)
                        .Scope(descriptor.Lifetime);

                }
                else if (descriptor.ImplementationFactory != null)
                {
                    builder.Register((scope) =>
                            descriptor.ImplementationFactory(scope.Resolve<IServiceProvider>(null))
                        )
                        .As(descriptor.ServiceType)
                        .Scope(descriptor.Lifetime);
                }
                else
                {
                    builder
                        .Register(descriptor.ServiceType)
                        .UsingInstance(descriptor.ImplementationInstance)
                        .As(descriptor.ServiceType);
                }
            }
        }
    }


}