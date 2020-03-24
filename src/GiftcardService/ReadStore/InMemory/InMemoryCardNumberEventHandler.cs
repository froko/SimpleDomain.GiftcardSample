namespace GiftcardService.ReadStore.InMemory
{
    using System.Threading.Tasks;

    public class InMemoryCardNumberEventHandler
    {
        private readonly IReadStore readStore;

        public InMemoryCardNumberEventHandler(IReadStore readStore)
        {
            this.readStore = readStore;
        }

        public Task HandleAsync(GiftcardCreated message)
        {
            return Task.Run(() => this.readStore.CardNumbers.Add(message.CardNumber));
        }
    }
}