namespace GiftcardService
{
    using System;

    using FluentAssertions.Collections;

    using GiftcardService.ReadStore;

    public static class FluentTestingExtensions
    {
        public static void HaveValidReadModel(
            this GenericCollectionAssertions<GiftcardOverview> readStore,
            int cardNumber,
            decimal balance,
            DateTime validUntil,
            GiftcardStatus giftcardStatus)
        {
            readStore.Contain(giftcardOverview =>
                giftcardOverview.CardNumber == cardNumber &&
                giftcardOverview.CurrentBalance == balance &&
                giftcardOverview.ValidUntil == validUntil &&
                giftcardOverview.Status == giftcardStatus);
        }

        public static void HaveValidReadModel(
            this GenericCollectionAssertions<GiftcardTransaction> readStore,
            int cardNumber,
            string eventText,
            decimal balance,
            decimal amount)
        {
            readStore.Contain(giftcardTransaction =>
                giftcardTransaction.CardNumber == cardNumber &&
                giftcardTransaction.ValutaDate == DateTime.Today &&
                giftcardTransaction.Event == eventText &&
                giftcardTransaction.Balance == balance &&
                giftcardTransaction.Amount == amount);
        }
    }
}