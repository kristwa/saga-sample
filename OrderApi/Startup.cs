using System.Text.Json;
using Common.Events;
using ET.Common;
using ET.Common.Logging;
using MassTransit;
using Serilog;

namespace OrderApi;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCommonServices(Configuration);

        services.AddSwaggerGen();

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.UsingAzureServiceBus((_, cfg) =>
            {
                cfg.Host(Configuration.GetConnectionString("AzureServiceBus"));
                cfg.Send<CardsProcessed>(s => s.UseSessionIdFormatter(c => c.Message.Order.OrderId.ToString()));
                cfg.Send<ProcessGiftcardItemRequestCreated>(s => s.UseSessionIdFormatter(c => c.Message.GiftcardItem.ItemId.ToString()));
                // cfg.Publish<IOrderCreated>(s => );
                //
                // cfg.RequiresDuplicateDetection = true;
            });
        });
        
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseSwagger();
        app.UseSwaggerUI();
        
        app.UseSerilogRequestLogging(opts => {
            opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
            opts.GetLevel = LogHelper.ExcludeHealthChecks;
        });

        app.UseEndpoints(endpoints =>
        {

            endpoints.MapControllers();
        });
    }
}