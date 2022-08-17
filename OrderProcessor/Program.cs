using System.Reflection;
using Common.Commands;
using Common.Events;
using ET.Common.Configuration;
using ET.Common.Logging;
using MassTransit;
using Microsoft.Extensions.Hosting;
using OrderProcessor.Consumers;
using OrderProcessor.Consumers.Card;
using OrderProcessor.Consumers.Order;
using OrderProcessor.Services.Card;
using OrderProcessor.Services.Order;
using OrderProcessor.State;
using Serilog;

Log.Logger = LogHelper.CreateInitialLogger();

try
{
    Log.Information("Starting up, Version {Version}", Assembly.GetExecutingAssembly().GetName().Version);
    await CreateHostBuilder(args).Build().RunAsync();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Start-up failed");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog(LogHelper.ConfigureLogger)
        .ConfigureServices((hostContext, services) =>
        {
            var configuration = ConfigurationHelper.GetCommonConfiguration<Program>();
            
            services.AddMassTransit(x =>
            {
                x.AddServiceBusMessageScheduler();
                x.SetKebabCaseEndpointNameFormatter();

                x.AddConsumer<CreateCardInCasConsumer>();
                x.AddConsumer<CreateGiftcardPrintConsumer>();
                x.AddConsumer<DepositCardConsumer>();
                x.AddConsumer<SendGiftcardPrintConsumer>();
                x.AddConsumer<VerifyDepositConsumer>();
                x.AddConsumer<CardFinalizerConsumer>();

                x.AddConsumer<CreateReceiptConsumer>();
                x.AddConsumer<SendReceiptConsumer>();
                
                x.AddConsumer<PaymentVerifierConsumer>();
                
                
                x.AddSagaStateMachine<CardProcessorStateMachine, CardProcessingState, CardProcessorSagaDefinition>()
                    .MessageSessionRepository();

                x.AddSagaStateMachine<OrderProcessorStateMachine, OrderProcessorState, OrderProcessorSagaDefinition>()
                    .MessageSessionRepository();
             
                
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(configuration.GetConnectionString("AzureServiceBus"));
                    cfg.UseServiceBusMessageScheduler();
                    
                    cfg.Send<CreateCardInCas>(s => s.UseSessionIdFormatter(c => c.Message.GiftcardItem.ItemId.ToString()));
                    cfg.Send<DepositCard>(s => s.UseSessionIdFormatter(c => c.Message.GiftcardItem.ItemId.ToString()));
                    cfg.Send<VerifyDeposit>(s => s.UseSessionIdFormatter(c => c.Message.GiftcardItem.ItemId.ToString()));
                    cfg.Send<CreateGiftcardPrint>(s => s.UseSessionIdFormatter(c => c.Message.GiftcardItem.ItemId.ToString()));
                    cfg.Send<SendGiftcard>(s => s.UseSessionIdFormatter(c => c.Message.GiftcardItem.ItemId.ToString()));
                    cfg.Send<FinalizeOrder>(s => s.UseSessionIdFormatter(c => c.Message.GiftcardItem.ItemId.ToString()));
                    
                    cfg.Send<CreateReceipt>(s => s.UseSessionIdFormatter(c => c.Message.Order.OrderId.ToString()));
                    cfg.Send<IReceiptGenerated>(s => s.UseSessionIdFormatter(c => c.Message.Order.OrderId.ToString()));
                    cfg.Send<SendReceipt>(s => s.UseSessionIdFormatter(c => c.Message.Order.OrderId.ToString()));
                    
                    cfg.ConfigureEndpoints(context);

                });
            });
        });


        