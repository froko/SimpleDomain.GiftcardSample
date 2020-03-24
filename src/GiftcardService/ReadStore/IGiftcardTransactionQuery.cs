namespace GiftcardService.ReadStore
{
    using System;
    using System.Collections.Generic;

    public interface IGiftcardTransactionQuery
    {
        IEnumerable<GiftcardTransaction> Find(Guid cardId);

        IEnumerable<GiftcardTransaction> Find(int cardNumber);
    }
}