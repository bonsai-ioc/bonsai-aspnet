namespace Bonsai.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using LifeStyles;
    using Microsoft.Extensions.DependencyInjection;
    using Registry;

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

            var items = descriptors.GroupBy(x => x.ServiceType).Select(grp => new
            {
                descriptors = grp,
                count = grp.Count()
            }).OrderByDescending(x=> x.count).ToList();
            
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
                        .As(descriptor.ServiceType)
                        .Scope(descriptor.Lifetime);
                }
                else
                {
                    builder
                        .Register(descriptor.ServiceType)
                        .UsingInstance(descriptor.ImplementationInstance)
                        .As(descriptor.ServiceType);
                        //.Scope(descriptor.Lifetime);
                }
            }
        }

        
    }


    
    public static class BuilderExtensions 
    {
        public static MultipleFluentRegistration RegisterAll(this ContainerBuilder builder, IEnumerable<Type> types)
        {
            var reg = new MultipleFluentRegistration(types);
            foreach (var registration in reg.Registrations)
            {
                builder.RegisterContract(registration);
            }

            return reg;
        }
    }

    public class MultipleFluentRegistration
    {
        private readonly List<FluentBuilder> _builders;

        public List<Registration> Registrations { get; private set; }
        
        public MultipleFluentRegistration(IEnumerable<Type> types)
        {
            Registrations = new List<Registration>();
            _builders = types.Select(x=>
            {
                var reg = new Registration();
                Registrations.Add(reg);
                return new FluentBuilder(reg, x);
            }).ToList();
        }

        public MultipleFluentRegistration As(Type type)
        {
            foreach (var builder in _builders)
            {
                builder.As(type, Guid.NewGuid().ToString());
            }

            return this;
        }

        public MultipleFluentRegistration Scoped<T>() where T : ILifeSpan, new()
        {
            foreach (var builder in _builders)
            {
                builder.Scoped<T>();
            }

            return this;
        }
        
        
    }
}