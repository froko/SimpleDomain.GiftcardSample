namespace GiftcardService.ReadStore
{
    using System;

    public class GiftcardTransaction
    {
        public Guid CardId { get; set; }

        public int CardNumber { get; set; }

        public DateTime ValutaDate { get; set; }

        public string Event { get; set; }

        public decimal Balance { get; set; }

        public decimal Amount { get; set; }

        public int Revision { get; set; }
    }
}