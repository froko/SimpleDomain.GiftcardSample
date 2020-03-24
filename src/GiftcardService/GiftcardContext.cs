namespace GiftcardService
{
    using System.Threading.Tasks;
    using GiftcardService.Domain;
    using GiftcardService.ReadStore;
    using GiftcardService.ReadStore.InMemory;
    using SimpleDomain;
    using SimpleDomain.Bus;

    public class GiftcardContext : IBoundedContext
    {
        private readonly ICardNumberQuery cardNumberQuery;
        private readonly InMemoryCardNumberEventHandler cardNumberEventHandler;
        private readonly InMemoryGiftcardOverviewEventHandler giftcardOverviewEventHandler;
        private readonly InMemoryGiftcardTransactionEventHandler giftcardTransactionEventHandler;

        public GiftcardContext(IReadStore readStore)
        {
            this.cardNumberQuery = new InMemoryCardNumberQuery(readStore);
            this.cardNumberEventHandler = new InMemoryCardNumberEventHandler(readStore);
            this.giftcardOverviewEventHandler = new InMemoryGiftcardOverviewEventHandler(readStore);
            this.giftcardTransactionEventHandler = new InMemoryGiftcardTransactionEventHandler(readStore);
        }


        public string Name => "Giftcards";

        private IEventSourcedRepository Repository { get; set; }

        public void Configure(
            ISubscribeMessageHandlers configuration,
            IFeatureSelector featureSelector,
            IDeliverMessages bus,
            IEventSourcedRepository repository)
        {
            this.Repository = repository;

            configuration.SubscribeCommandHandler<CreateGiftcard>(this.HandleAsync);
            configuration.SubscribeCommandHandler<ActivateGiftcard>(this.HandleAsync);
            configuration.SubscribeCommandHandler<RedeemGiftcard>(this.HandleAsync);
            configuration.SubscribeCommandHandler<LoadGiftcard>(this.HandleAsync);

            configuration.SubscribeEventHandler<GiftcardCreated>(this.cardNumberEventHandler.HandleAsync);

            configuration.SubscribeEventHandler<GiftcardCreated>(this.giftcardOverviewEventHandler.HandleAsync);
            configuration.SubscribeEventHandler<GiftcardActivated>(this.giftcardOverviewEventHandler.HandleAsync);
            configuration.SubscribeEventHandler<GiftcardRedeemed>(this.giftcardOverviewEventHandler.HandleAsync);
            configuration.SubscribeEventHandler<GiftcardLoaded>(this.giftcardOverviewEventHandler.HandleAsync);

            configuration.SubscribeEventHandler<GiftcardCreated>(this.giftcardTransactionEventHandler.HandleAsync);
            configuration.SubscribeEventHandler<GiftcardActivated>(this.giftcardTransactionEventHandler.HandleAsync);
            configuration.SubscribeEventHandler<GiftcardRedeemed>(this.giftcardTransactionEventHandler.HandleAsync);
            configuration.SubscribeEventHandler<GiftcardLoaded>(this.giftcardTransactionEventHandler.HandleAsync);
        }

        private async Task HandleAsync(CreateGiftcard command)
        {
            if (this.cardNumberQuery.IsAlreadyInUse(command.CardNumber))
            {
                throw new GiftcardException($"A giftcard with number {command.CardNumber} already exists.");
            }

            var giftcard = new Giftcard(
                command.CardNumber,
                command.InitialBalance,
                command.ValidUntil);

            await this.Repository.SaveAsync(giftcard).ConfigureAwait(false);
        }

        private async Task HandleAsync(ActivateGiftcard command)
        {
            var giftcard = await this.Repository
                .GetByIdAsync<Giftcard>(command.CardId)
                .ConfigureAwait(false);

            giftcard.Activate();

            await this.Repository.SaveAsync(giftcard).ConfigureAwait(false);
        }

        private async Task HandleAsync(RedeemGiftcard command)
        {
            var giftcard = await this.Repository
                .GetByIdAsync<Giftcard>(command.CardId)
                .ConfigureAwait(false);

            giftcard.Redeem(command.Amount);

            await this.Repository.SaveAsync(giftcard).ConfigureAwait(false);
        }

        private async Task HandleAsync(LoadGiftcard command)
        {
            var giftcard = await this.Repository
                .GetByIdAsync<Giftcard>(command.CardId)
                .ConfigureAwait(false);

            giftcard.Load(command.Amount);

            await this.Repository.SaveAsync(giftcard).ConfigureAwait(false);
        }
    }
}