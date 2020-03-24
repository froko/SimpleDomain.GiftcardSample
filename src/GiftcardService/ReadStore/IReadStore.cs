namespace GiftcardService.ReadStore
{
    using System.Collections.Generic;

    public interface IReadStore
    {
        IList<int> CardNumbers { get; }

        IList<GiftcardOverview> GiftcardOverviews { get; }

        IList<GiftcardTransaction> GiftcardTransactions { get; } 
    }
}