# Start It Up!
I've coded this little utility with a specific purpose: to help me keep and execute the application startup code in a nice and organized manner. Instead of shoving up all the init code in Application_Start, Startup etc, I can use specific classes for specific initialization.

In this way, I can concentrate only on the details I need to care of and I don't have to scroll 3 pages to find where to insert another startup code.

Version: [2.0.0](https://github.com/sapiens/startitup/wiki/ChangeLog)

## License

Apache 2.0

## Usage

### Install from Nuget

`Install-Package StartItUp`

### Create a settings object

```csharp
public class AppSettings:StartupContext
{
    public AppSettings()
    {
    }
    
    public bool IsDebug {get;set;}
    
    public string DbConnection {get;set;}
}

```

It's not required to inherit from `StartupContext` but it's recommended, because it acts as a value bag that can be used to pass values between startup tasks.

### Create startup tasks

The naming convention is "ConfigTask_[order]_[TaskName]" e.g `ConfigTask_1_ORM` . One exception is the log configuration class which should be `ConfigureLogging` and which is always executed first (if it exists, ofc). Each of these classes should have a public method called `Run` that can have 1 or 0 parameters. The only parameter that can be passed is the configuration object e.g `AppSettings`.

```csharp
 public class ConfigureLogging
    {
        public void Run(AppSettings cfg)
        {
            var tpl = "{Timestamp:G} |{Level}|{Message}{NewLine:l}{Exception:l}";

            var logConfig = new LoggerConfiguration();
            if (cfg.IsDebug)
            {
                logConfig.MinimumLevel.Debug();
            }
            else
            {
                logConfig.MinimumLevel.Information();
            }

            logConfig
                .WriteTo.Trace(outputTemplate: tpl,restrictedToMinimumLevel:LogEventLevel.Debug)
                .WriteTo.RollingFile(cfg.LogDirectory + "log-{Date}.log", outputTemplate: tpl, restrictedToMinimumLevel: LogEventLevel.Warning);
            LogManager.SetWriter(new SerilogAdapter(logConfig.CreateLogger()));
        }
    }
    
    //makes use of StartItUp.Autofac
    public class ConfigTask_0_RegisterServices
    {

        public void Run(AppSettings cfg)
        {
            cfg.ConfigureContainer(cb =>
            {
                cb.RegisterServices(asm: cfg.AppAssemblies, cfgServices: svc => svc.Add("Services"));
            });
        }
    }
    
    //let's configure Nancy
    public class ConfigTask_3_WebFramework
    {
        public void Run(AppSettings cfg)
        {
            var owin = cfg.Owin;
            owin.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                CookieName = "_dfauth",
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromDays(7)
            }, PipelineStage.Authenticate);
            owin.UseNancy(opt => opt.Bootstrapper = new NancyBoot(cfg));
            owin.UseStageMarker(PipelineStage.MapHandler);
        }
    }
```

### Start it up!

In the entry point method of your app

```csharp
 
 //without deps
 StartIt.Up<AppSettings>();

//if you want to pass some info e.g for a OWIN web app
 public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            StartIt.Up(new AppSettings(app));
        }
    }
 
```
