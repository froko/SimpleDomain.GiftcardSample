namespace GiftcardService.ReadStore.InMemory
{
    using System.Linq;

    public class InMemoryCardNumberQuery : ICardNumberQuery
    {
        private readonly IReadStore readStore;

        public InMemoryCardNumberQuery(IReadStore readStore)
        {
            this.readStore = readStore;
        }

        public bool IsAlreadyInUse(int cardNumber)
        {
            return this.readStore.CardNumbers.Any(n => n == cardNumber);
        }
    }
}