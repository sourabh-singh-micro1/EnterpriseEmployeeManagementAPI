using Serilog;

namespace EnterpriseEmployeeManagementAPI.Logging;

public static class LoggingConfiguration
{
    public static WebApplicationBuilder AddStructuredLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "EnterpriseEmployeeManagementAPI")
                .WriteTo.Console());

        return builder;
    }
}
