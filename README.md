# Stdout Logger

A plain Stdout logger with Http context capturing.


## Usage

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }


    public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        => WebHost.CreateDefaultBuilder(args)
            .UseKestrel()
            .UseStartup<Startup>()
            .UseDefaultServiceProvider)
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.ClearProviders()
                    .AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                logging.AddStdOutLogger(options =>
                {
                    options.IncludeScopes = false;
                    options.RequestIdHeader = Constants.DefaultRequestIdHeader;
                    options.UseUtcTimestamp = true;
                });
            })
            .UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "true");
}


public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        ...
        
        app.UseHttpContextLogging(options => options.IgnoredPaths = new HashSet<string> {"/health"});
        
        ...
    }
}
```


## Automation

New package version is automatically published to [github packages](https://github.com/features/packages) after changes in the master branch.


## Dependencies

The project depends on following packages: 
* `Newtonsoft.Json`
