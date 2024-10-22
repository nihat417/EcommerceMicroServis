using Ecommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Data.Common;

namespace Ecommerce.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>
            (this IServiceCollection services,IConfiguration config,string fileName) where TContext: DbContext
        {
            //Add generic database context
            services.AddDbContext<TContext>(op => op.UseSqlServer(
                config.GetConnectionString(""),sqlServerOptionsAction => 
                    sqlServerOptionsAction.EnableRetryOnFailure()
            ));

            //serilog config
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path:$"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate:"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{level:u3}] {message:lj}{NewLine}{Exception}",
                rollingInterval:RollingInterval.Day)
                .CreateLogger();

            //add jwt auth scheme
            JwtAuthScheme.AddJwtAuthenticationScheme(services,config);

            return services;
        }

        public static IApplicationBuilder UseSharedPolicy(this IApplicationBuilder app)
        {
            //use global exception
            app.UseMiddleware<GlobalException>();

            //register middleware to block outside api calls
            app.UseMiddleware<ListenToOnlyApiGateway>();
            return app;
        }
    }
}
