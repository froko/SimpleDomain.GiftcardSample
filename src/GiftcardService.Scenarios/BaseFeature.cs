namespace GiftcardService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using GiftcardService.Domain;
    using GiftcardService.ReadStore;
    using GiftcardService.ReadStore.InMemory;

    using SimpleDomain;

    public abstract class BaseFeatures : IDisposable
    {
        private readonly ExecutionContext executionContext;

        protected BaseFeatures()
        {
            var readStore = new InMemoryReadStore();
            var compositionRoot = new CompositionRoot();

            compositionRoot.Register(new GiftcardContext(readStore));
            compositionRoot.ConfigureJitney()
                .DefineLocalEndpointAddress("gc.sample")
                .MapContracts(typeof(CreateGiftcard).Assembly).ToMe();

            this.executionContext = compositionRoot.Start();

            this.OverviewQuery = new InMemoryGiftcardOverviewQuery(readStore);
            this.TransactionQuery = new InMemoryGiftcardTransactionQuery(readStore);
        }

        protected IGiftcardOverviewQuery OverviewQuery { get; }

        protected IGiftcardTransactionQuery TransactionQuery { get; }

        protected IDeliverMessages Bus => this.executionContext.Bus;

        private IEventStore EventStore => this.executionContext.EventStore;

        public void Dispose()
        {
            this.executionContext.Dispose();
        }

        protected async Task PrepareEventsAsync(Guid cardId, params IEvent[] events)
        {
            var expectedVersion = events.Length - 1;
            var version = 0;

            var versionableEvents = events.Select(@event => new VersionableEvent(@event).With(version++));

            using (var eventStream = await this.EventStore.OpenStreamAsync<Giftcard>(cardId).ConfigureAwait(false))
            {
                await eventStream
                    .SaveAsync(versionableEvents, expectedVersion, new Dictionary<string, object>())
                    .ConfigureAwait(false);
            }
        }
    }
}