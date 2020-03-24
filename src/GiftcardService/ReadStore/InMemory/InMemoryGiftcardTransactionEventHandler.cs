namespace GiftcardService.ReadStore.InMemory
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using SimpleDomain;

    public class InMemoryGiftcardTransactionEventHandler :
        IHandleAsync<GiftcardCreated>,
        IHandleAsync<GiftcardActivated>,
        IHandleAsync<GiftcardRedeemed>,
        IHandleAsync<GiftcardLoaded>
    {
        private readonly IReadStore readStore;

        public InMemoryGiftcardTransactionEventHandler(IReadStore readStore)
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
            this.readStore.GiftcardTransactions.Add(new GiftcardTransaction
            {
                CardId = message.CardId,
                CardNumber = message.CardNumber,
                ValutaDate = DateTime.Today,
                Event = message.GetType().Name,
                Balance = message.InitialBalance,
                Amount = 0,
                Revision = 0
            });
        }

        private void Handle(GiftcardActivated message)
        {
            var previousTransaction = this.readStore.GiftcardTransactions.Last(g => g.CardId == message.CardId);
            var newRevision = previousTransaction.Revision;
            newRevision++;

            this.readStore.GiftcardTransactions.Add(new GiftcardTransaction
            {
                CardId = message.CardId,
                CardNumber = previousTransaction.CardNumber,
                ValutaDate = DateTime.Today,
                Event = message.GetType().Name,
                Balance = previousTransaction.Balance,
                Amount = previousTransaction.Amount,
                Revision = newRevision
            });
        }

        private void Handle(GiftcardRedeemed message)
        {
            var previousTransaction = this.readStore.GiftcardTransactions.Last(g => g.CardId == message.CardId);
            var newRevision = previousTransaction.Revision;
            newRevision++;

            this.readStore.GiftcardTransactions.Add(new GiftcardTransaction
            {
                CardId = message.CardId,
                CardNumber = previousTransaction.CardNumber,
                ValutaDate = DateTime.Today,
                Event = message.GetType().Name,
                Balance = previousTransaction.Balance - message.Amount,
                Amount = message.Amount,
                Revision = newRevision
            });
        }

        private void Handle(GiftcardLoaded message)
        {
            var previousTransaction = this.readStore.GiftcardTransactions.Last(g => g.CardId == message.CardId);
            var newRevision = previousTransaction.Revision;
            newRevision++;

            this.readStore.GiftcardTransactions.Add(new GiftcardTransaction
            {
                CardId = message.CardId,
                CardNumber = previousTransaction.CardNumber,
                ValutaDate = DateTime.Today,
                Event = message.GetType().Name,
                Balance = previousTransaction.Balance + message.Amount,
                Amount = message.Amount,
                Revision = newRevision
            });
        }
    }
}