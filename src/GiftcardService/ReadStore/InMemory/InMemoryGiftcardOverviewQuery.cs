namespace GiftcardService.ReadStore.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class InMemoryGiftcardOverviewQuery : IGiftcardOverviewQuery
    {
        private readonly IReadStore readStore;

        public InMemoryGiftcardOverviewQuery(IReadStore readStore)
        {
            this.readStore = readStore;
        }

        public IEnumerable<GiftcardOverview> FindAll()
        {
            return this.readStore.GiftcardOverviews;
        }

        public GiftcardOverview Find(Guid cardId)
        {
            return this.readStore.GiftcardOverviews.SingleOrDefault(g => g.CardId == cardId);
        }

        public GiftcardOverview Find(int cardNumber)
        {
            return this.readStore.GiftcardOverviews.SingleOrDefault(g => g.CardNumber == cardNumber);
        }
    }
}