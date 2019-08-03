# Bonsai IoC AspNetCore

please note this is a new project. if you find any issues please let us know and event submit a PR.

## Features

* ASPNET WebApi Core 3.x support 
* ASPNET WebApi Core 2.x support
* support for the Microsoft.Extensions.DependencyInjection Abstractions, with added support of Bonsai Features

## packages in this Repo

* Bonsai.Ioc.Extensions.DependencyInjection - has all the DependencyInjection support, no dependency on ASPNET
* Bonsai.Ioc.Extensions.Hosting - support on the IHostBuilder ready for .NET 3.x

As mentioned the 2 main version of ASPNET Core Webapi are supported (it should support all ASPNET but just not tested it) below is how you can set Bonsai up:

# 3.x

install the "Bonsai.Ioc.Extensions.DependencyInjection" and "Bonsai.Ioc.Extensions.Hosting" packages

**Program.cs**

at the moment we have his working with Kestrel 

```
using Bonsai.Extensions.Hosting;

public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //Add here:
                .UseBonsai()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel();
                    webBuilder.UseStartup<Startup>();

                });
    }
```

**Startup.cs**

```
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpContextAccessor(); //consider adding this
        }


        public void ConfigureContainer(ContainerBuilder builder)
        {
            //setup the container as normal
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
```



# 2.x

**Program.cs**

install the "Bonsai.Extensions.DependencyInjection" package

```
using Bonsai.Extensions.DependencyInjection;

public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            //Add Here:
            .ConfigureServices(services => services.UseBonsai())
            .UseStartup<Startup>();
}
```

**Startup.cs**

```
using Bonsai;
using Bonsai.LifeStyles;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    }
    
    //setup bonsai here
    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.Register<Service1>().As<IService1>().Scoped<Singleton>();

        builder.SetupModules(
            new LoggingModule(),
            new ApplicationModule(), 
            new DataAccessModule(),
            new MiddlewareModule());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseMvc();
    }
}
```