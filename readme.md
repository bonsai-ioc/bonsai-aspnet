# Bonsai IoC AspNetCore

please note this is a new project. if you find any issues please let us know and event submit a PR.

# 2.x

**Program.cs**

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
            .ConfigureServices(services => services.AddBonsai())
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