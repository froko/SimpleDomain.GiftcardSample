namespace GiftcardService.ReadStore.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class InMemoryGiftcardTransactionQuery : IGiftcardTransactionQuery
    {
        private readonly IReadStore readStore;

        public InMemoryGiftcardTransactionQuery(IReadStore readStore)
        {
            this.readStore = readStore;
        }

        public IEnumerable<GiftcardTransaction> Find(Guid cardId)
        {
            return this.readStore.GiftcardTransactions.Where(t => t.CardId == cardId);
        }

        public IEnumerable<GiftcardTransaction> Find(int cardNumber)
        {
            return this.readStore.GiftcardTransactions.Where(t => t.CardNumber == cardNumber);
        }
    }
}