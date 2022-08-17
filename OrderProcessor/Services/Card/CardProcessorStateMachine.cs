using Common.Commands;
using Common.DTO;
using Common.Events;
using MassTransit;
using OrderProcessor.State;
using Serilog;

namespace OrderProcessor.Services.Card;

public class CardProcessorStateMachine : MassTransitStateMachine<CardProcessingState>
{
    public CardProcessorStateMachine()
    {
        InstanceState(x => x.CurrentState);
        // this.State(() => this.Processing);
        this.ConfigureCorrelationIds();
        
        Initially(
            When(ProcessGiftcardRequested)
                .TransitionTo(Processing)
                .Then(p => Log.Information("Card Saga Step 1: Create Card ({Correlation})", p.Message.CorrelationId))
                .PublishAsync(context => context.Init<CreateCardInCas>(new CreateCardInCas() { CorrelationId = context.Message.CorrelationId, GiftcardItem = context.Message.GiftcardItem }))
            );

        During(Processing,
            When(CardCreated)
                .Then(context => Log.Information("Card Saga Step 2: Deposit card"))
                .PublishAsync(context => context.Init<DepositCard>(new DepositCard()
                    { CorrelationId = context.Message.CorrelationId, GiftcardItem = context.Message.GiftcardItem })));

        During(Processing,
            When(CardDeposited)
                .Then(c => Log.Information("Card Saga Step 3: Verify Deposit for Card {cardNr}", c.Message.GiftcardItem.CardNumber))
                .PublishAsync(ctx => ctx.Init<VerifyDeposit>(new VerifyDeposit() { CorrelationId = ctx.Message.CorrelationId, GiftcardItem = ctx.Message.GiftcardItem})));
        
        During(Processing,
            When(CardDepositVerified)
                .Then(ctx => Log.Information("Card Saga Step 4: Create print design"))
                .PublishAsync(ctx => ctx.Init<CreateGiftcardPrint>(new CreateGiftcardPrint()
                {
                    CorrelationId = ctx.Message.CorrelationId,
                    GiftcardItem = ctx.Message.GiftcardItem
                }))
            );
        
        During(Processing,
            When(CardDesignCreated)
                .Then(ctx => Log.Information("Card Saga Step 5: Send giftcard {CardNo}", ctx.Message.GiftcardItem.CardNumber))
                .PublishAsync(ctx => ctx.Init<SendGiftcard>(new SendGiftcard()
                {
                    CorrelationId = ctx.Message.CorrelationId,
                    GiftcardItem = ctx.Message.GiftcardItem
                }))
            );

        During(Processing,
            When(GiftcardSent)
                .Then(ctx => Log.Information("Card Saga Step 6: Finalize giftcard {CardNo}", ctx.Message.GiftcardItem.CardNumber))
                .PublishAsync(ctx => ctx.Init<FinalizeOrder>(new FinalizeOrder()
                {
                    CorrelationId = ctx.Message.CorrelationId,
                    GiftcardItem = ctx.Message.GiftcardItem
                }))
                .Finalize()
        );
    }

    private void ConfigureCorrelationIds()
    {
        Event(() => ProcessGiftcardRequested,
            x => x.CorrelateById(c => c.Message.CorrelationId).SelectId(c => c.Message.CorrelationId));
        Event(() => CardCreated, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => CardDeposited, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => CardDepositVerified, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => CardDesignCreated, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => GiftcardSent, x => x.CorrelateById(c => c.Message.CorrelationId));
    }
    
    // private EventActivityBinder<CardProcessingState, IOrderCreated> SetOrderCreatedHandler() => 
    //     When(OrderCreated).Then(c => this.UpdateSagaState(c.Instance,  ))

    
    
    public MassTransit.State Processing { get; set; }
    public Event<ProcessGiftcardItemRequestCreated> ProcessGiftcardRequested { get; private set; }
    public Event<ICardCreated> CardCreated { get; private set; }
    public Event<ICardDeposited> CardDeposited { get; private set; }
    public Event<ICardDepositVerified> CardDepositVerified { get; set; }
    public Event<ICardDesignPrintCreated> CardDesignCreated { get; private set; }
    public Event<IGiftcardSent> GiftcardSent { get; private set; }

    // Trigger: Request to (create and) fill a giftcard
    // Steps:
    // 1. Create a card in cas if cardnumber is not already provided
    // 2. Deposit money into card
    // 3. Verify money is deposited
    // 4. Create print design if applicable
    // 5. Send giftcard if applicable
    // 5. Check if all card request in order has been processed
}