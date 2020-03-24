namespace GiftcardService.ReadStore.InMemory
{
    using System.Collections.Generic;

    public class InMemoryReadStore : IReadStore
    {
        public InMemoryReadStore()
        {
            this.CardNumbers = new List<int>();
            this.GiftcardOverviews = new List<GiftcardOverview>();
            this.GiftcardTransactions = new List<GiftcardTransaction>();
        }

        public IList<int> CardNumbers { get; private set; }

        public IList<GiftcardOverview> GiftcardOverviews { get; private set; }

        public IList<GiftcardTransaction> GiftcardTransactions { get; private set; }
    }
}