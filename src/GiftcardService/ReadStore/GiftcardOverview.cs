namespace GiftcardService.ReadStore
{
    using System;

    public class GiftcardOverview
    {
        public Guid CardId { get; set; }

        public int CardNumber { get; set; }

        public DateTime ValidUntil { get; set; }

        public decimal CurrentBalance { get; set; }

        public GiftcardStatus Status { get; set; }
    }
}