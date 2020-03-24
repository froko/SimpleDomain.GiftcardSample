namespace GiftcardService.ReadStore.InMemory
{
    using System.Linq;
    using System.Threading.Tasks;
    using SimpleDomain;

    public class InMemoryGiftcardOverviewEventHandler :
        IHandleAsync<GiftcardCreated>,
        IHandleAsync<GiftcardActivated>,
        IHandleAsync<GiftcardRedeemed>,
        IHandleAsync<GiftcardLoaded>
    {
        private readonly IReadStore readStore;

        public InMemoryGiftcardOverviewEventHandler(IReadStore readStore)
        {
            this.readStore = readStore;
        }

        public Task HandleAsync(GiftcardCreated message)
        {
            return Task.Run(() => this.Handle(message));
        }

        public Task HandleAsync(GiftcardActivated message)
        {
            return Task.Run(() => this.Handle(message));
        }

        public Task HandleAsync(GiftcardRedeemed message)
        {
            return Task.Run(() => this.Handle(message));
        }

        public Task HandleAsync(GiftcardLoaded message)
        {
            return Task.Run(() => this.Handle(message));
        }

        private void Handle(GiftcardCreated message)
        {
            this.readStore.GiftcardOverviews.Add(new GiftcardOverview
            {
                CardId = message.CardId,
                CardNumber = message.CardNumber,
                CurrentBalance = message.InitialBalance,
                ValidUntil = message.ValidUntil,
                Status = GiftcardStatus.Deactivated
            });
        }

        private void Handle(GiftcardActivated message)
        {
            var giftcard = this.readStore.GiftcardOverviews.Single(g => g.CardId == message.CardId);
            giftcard.Status = GiftcardStatus.Activated;
        }

        private void Handle(GiftcardRedeemed message)
        {
            var giftcard = this.readStore.GiftcardOverviews.Single(g => g.CardId == message.CardId);
            giftcard.CurrentBalance -= message.Amount;
        }

        private void Handle(GiftcardLoaded message)
        {
            var giftcard = this.readStore.GiftcardOverviews.Single(g => g.CardId == message.CardId);
            giftcard.CurrentBalance += message.Amount;
        }
    }
}