namespace GiftcardService.ReadStore
{
    using System;
    using System.Collections.Generic;

    public interface IGiftcardOverviewQuery
    {
        IEnumerable<GiftcardOverview> FindAll();

        GiftcardOverview Find(Guid cardId);

        GiftcardOverview Find(int cardNumber);
    }
}