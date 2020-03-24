# Giftcard Sample

A sample project showing the capabilities of SimpleDomain.

![Supported .NET Core versions: 3.1+](https://img.shields.io/badge/Core-3.1+-blue.svg)
![Supported .NET Standard versions: 2.0+](https://img.shields.io/badge/Standard-2.0+-blue.svg)
[![Commitizen friendly](https://img.shields.io/badge/commitizen-friendly-brightgreen.svg)](http://commitizen.github.io/cz-cli/)

## Project Setup

```bash
mkdir GiftcardSample
cd GiftcardSample

mkdir src
cd src

dotnet new sln -n GiftcardSample
dotnet new classlib -n GiftcardService
dotnet new classlib -n GiftcardService.Contracts --language "F#"
dotnet new xunit -n GiftcardService.Facts
dotnet new xunit -n GiftcardService.Scenarios

dotnet sln ./GiftcardSample.sln add ./GiftcardService/GiftcardService.csproj
dotnet sln ./GiftcardSample.sln add ./GiftcardService.Contracts/GiftcardService.Contracts.fsproj
dotnet sln ./GiftcardSample.sln add ./GiftcardService.Facts/GiftcardService.Facts.csproj
dotnet sln ./GiftcardSample.sln add ./GiftcardService.Scenarios/GiftcardService.Scenarios.csproj

dotnet add ./GiftcardService/GiftcardService.csproj reference ./GiftcardService.Contracts/GiftcardService.Contracts.fsproj
dotnet add ./GiftcardService.Facts/GiftcardService.Facts.csproj reference ./GiftcardService.Contracts/GiftcardService.Contracts.fsproj
dotnet add ./GiftcardService.Scenarios/GiftcardService.Scenarios.csproj reference ./GiftcardService.Contracts/GiftcardService.Contracts.fsproj
dotnet add ./GiftcardService.Facts/GiftcardService.Facts.csproj reference ./GiftcardService/GiftcardService.csproj
dotnet add ./GiftcardService.Scenarios/GiftcardService.Scenarios.csproj reference ./GiftcardService/GiftcardService.csproj

cd GiftcardService
dotnet add package Microsoft.CodeQuality.Analyzers
dotnet add package SimpleDomain

cd..
cd GiftcardService.Contracts
dotnet add package SimpleDomain

cd ..
cd GiftcardService.Facts
dotnet add package Microsoft.CodeQuality.Analyzers
dotnet add package FluentAssertions
dotnet add package FakeItEasy

cd ..
cd GiftcardService.Scenarios
dotnet add package Microsoft.CodeQuality.Analyzers
dotnet add package FluentAssertions
dotnet add package FakeItEasy
dotnet add package XBehave
```

## Code Analysis Ruleset

We maintain a single code analysis ruleset in the root of the `src` folder. The ruleset is referenced in the C# project files:

```xml
<PropertyGroup>
  <CodeAnalysisRuleSet>..\GiftcardSample.ruleset</CodeAnalysisRuleSet>
</PropertyGroup>
```

## Message Contracts

Although our GiftcardSample is a single-proces, single-BoundedContext project, we do want to keep our message contracts (commands & events) in a seperate assembly. For defining messages, the F# record types are a perfect match since they can express a single message in just one or two lines of code:

### Commands

```fsharp
type CreateGiftcard = { CardNumber : int; InitialBalance : decimal; ValidUntil : DateTime } with interface ICommand
type ActivateGiftcard = { CardId : Guid; } with interface ICommand
type RedeemGiftcard = { CardId : Guid; Amount : decimal } with interface ICommand
type LoadGiftcard = { CardId : Guid; Amount : decimal } with interface ICommand
```

### Events

```fsharp
type GiftcardCreated = { CardId : Guid; CardNumber : int; InitialBalance : decimal; ValidUntil : DateTime }
    with interface IEvent

type GiftcardActivated = { CardId: Guid }
    with interface IEvent

type GiftcardRedeemed = { CardId: Guid; Amount : decimal }
    with interface IEvent

type GiftcardLoaded = { CardId: Guid; Amount : decimal }
    with interface IEvent
```

## Giftcard

The Giftcard is implemented as an event-sourced aggregate root. The base class is defined in the SimpleDomain framework and takes care of the following responsibilities:

- Mapping of event types to their corresponding apply methods
- Provisioning of a public Id property (`Guid`)
- Provisioning of an incrementing numeric Version property
- Recreation of an instance from an event history (a series of events)
- Ability to create a snapshot (needs to be overridden, if used)
- Ability to load from a snapshot (needs to be overridden, if used)

Here's the implementation. The checks for invariants are ommited for brevity:

```csharp
public class Giftcard : StaticEventSourcedAggregateRoot
    {
        private decimal balance;
        private DateTime validUntil;
        private bool isActivated;

        private Giftcard()
        {
            this.RegisterTransition<GiftcardCreated>(this.Apply);
            this.RegisterTransition<GiftcardActivated>(this.Apply);
            this.RegisterTransition<GiftcardRedeemed>(this.Apply);
            this.RegisterTransition<GiftcardLoaded>(this.Apply);
        }

        public Giftcard(int cardNumber, decimal initialBalance, DateTime validUntil) : this()
        {
            // Check invariants
            this.ApplyEvent(new GiftcardCreated(Guid.NewGuid(), cardNumber, initialBalance, validUntil));
        }

        public void Activate()
        {
            // Check invariants
            this.ApplyEvent(new GiftcardActivated(this.Id));
        }

        public void Redeem(decimal amount)
        {
            // Check invariants
            this.ApplyEvent(new GiftcardRedeemed(this.Id, amount));
        }

        public void Load(decimal amount)
        {
            // Check invariants
            this.ApplyEvent(new GiftcardLoaded(this.Id, amount));
        }

        private void Apply(GiftcardCreated @event)
        {
            this.Id = @event.CardId;
            this.balance = @event.InitialBalance;
            this.validUntil = @event.ValidUntil;
            this.isActivated = false;
        }

        private void Apply(GiftcardActivated @event)
        {
            this.isActivated = true;
        }

        private void Apply(GiftcardRedeemed @event)
        {
            this.balance -= @event.Amount;
        }

        private void Apply(GiftcardLoaded @event)
        {
            this.balance += @event.Amount;
        }
    }
```

## ReadStore

An event-sourced aggregate root needs an EventStore to save its emmitted events and to be recreated from. In our GiftcardSample we use the built-in InMemory store of the SimpleDomain framework. Since an EventStore is highly optimized for the command side of a CQRS based solution, it lacks of the performance we wish to have on the read side. Therefore, we need a ReadStore which is able to provide various projections of the same data base in a highly efficient manner.

In our GiftcardSample we use an InMemory ReadStore with the following projections:

- **CardNumbers**
  A single list of card numbers which is used to programatically maintain the uniqueness of these numbers
- **GiftcardOverviews**
  A single record for each Giftcard providing the actual state
- **GiftcardTransactions**
  All transactions of all Giftcards

Have a look at the `GiftcardService.ReadStore` namespace for more information.

## Bounded Context

The Giftcard aggregate root and the ReadStore build a single bounded context. The SimpleDomain framework provides an interface to express this context in class. Its main responsibility is to map all message types to their corresponding handlers:

```csharp
public class GiftcardContext : IBoundedContext
{
    private readonly ICardNumberQuery cardNumberQuery;
    private readonly InMemoryCardNumberEventHandler cardNumberEventHandler;
    private readonly InMemoryGiftcardOverviewEventHandler giftcardOverviewEventHandler;
    private readonly InMemoryGiftcardTransactionEventHandler giftcardTransactionEventHandler;

    public GiftcardContext(IReadStore readStore)
    {
        this.cardNumberQuery = new InMemoryCardNumberQuery(readStore);
        this.cardNumberEventHandler = new InMemoryCardNumberEventHandler(readStore);
        this.giftcardOverviewEventHandler = new InMemoryGiftcardOverviewEventHandler(readStore);
        this.giftcardTransactionEventHandler = new InMemoryGiftcardTransactionEventHandler(readStore);
    }

    public string Name => "Giftcards";

    private IEventSourcedRepository Repository { get; set; }

    public void Configure(
        ISubscribeMessageHandlers configuration,
        IFeatureSelector featureSelector,
        IDeliverMessages bus,
        IEventSourcedRepository repository)
    {
        this.Repository = repository;
        configuration.SubscribeCommandHandler<CreateGiftcard>(this.HandleAsync);
        configuration.SubscribeCommandHandler<ActivateGiftcard>(this.HandleAsync);
        configuration.SubscribeCommandHandler<RedeemGiftcard>(this.HandleAsync);
        configuration.SubscribeCommandHandler<LoadGiftcard>(this.HandleAsync);
        configuration.SubscribeEventHandler<GiftcardCreated>(this.cardNumberEventHandler.HandleAsync);
        configuration.SubscribeEventHandler<GiftcardCreated>(this.giftcardOverviewEventHandler.HandleAsync);
        configuration.SubscribeEventHandler<GiftcardActivated>(this.giftcardOverviewEventHandler.HandleAsync);
        configuration.SubscribeEventHandler<GiftcardRedeemed>(this.giftcardOverviewEventHandler.HandleAsync);
        configuration.SubscribeEventHandler<GiftcardLoaded>(this.giftcardOverviewEventHandler.HandleAsync);
        configuration.SubscribeEventHandler<GiftcardCreated>(this.giftcardTransactionEventHandler.HandleAsync);
        configuration.SubscribeEventHandler<GiftcardActivated>(this.giftcardTransactionEventHandler.HandleAsync);
        configuration.SubscribeEventHandler<GiftcardRedeemed>(this.giftcardTransactionEventHandler.HandleAsync);
        configuration.SubscribeEventHandler<GiftcardLoaded>(this.giftcardTransactionEventHandler.HandleAsync);
    }

    private async Task HandleAsync(CreateGiftcard command)
    {
        if (this.cardNumberQuery.IsAlreadyInUse(command.CardNumber))
        {
            throw new GiftcardException($"A giftcard with number {command.CardNumber} already exists.");
        }

        var giftcard = new Giftcard(
            command.CardNumber,
            command.InitialBalance,
            command.ValidUntil);

        await this.Repository.SaveAsync(giftcard).ConfigureAwait(false);
    }

    private async Task HandleAsync(ActivateGiftcard command)
    {
        var giftcard = await this.Repository
            .GetByIdAsync<Giftcard>(command.CardId)
            .ConfigureAwait(false);

        giftcard.Activate();

        await this.Repository.SaveAsync(giftcard).ConfigureAwait(false);
    }

    private async Task HandleAsync(RedeemGiftcard command)
    {
        var giftcard = await this.Repository
            .GetByIdAsync<Giftcard>(command.CardId)
            .ConfigureAwait(false);

        giftcard.Redeem(command.Amount);

        await this.Repository.SaveAsync(giftcard).ConfigureAwait(false);
    }

    private async Task HandleAsync(LoadGiftcard command)
    {
        var giftcard = await this.Repository
            .GetByIdAsync<Giftcard>(command.CardId)
            .ConfigureAwait(false);

        giftcard.Load(command.Amount);

        await this.Repository.SaveAsync(giftcard).ConfigureAwait(false);
    }
}
```

## CompositionRoot

The composition root combines the user code, expressed as bounded context, with the framework capabilities of SimpleDomain and does all the required wirings behind the scenes. A composition root can be fluently configured like this:

```csharp
var readStore = new InMemoryReadStore();
var compositionRoot = new CompositionRoot();

compositionRoot.Register(new GiftcardContext(readStore));
compositionRoot.ConfigureJitney()
    .DefineLocalEndpointAddress("gc.sample")
    .MapContracts(typeof(CreateGiftcard).Assembly).ToMe();
```

By starting the composition root, we get a disposable execution context, which gives us acces to the internal message bus and the EventStore:

```csharp
var executionContext = compositionRoot.Start();
var bus = executionContext.Bus;
var eventStore = executionContext.EventStore;
```

Have a look at the `BaseFeatures` class in the `GiftcardService.Scenarios` project to see the CompositionRoot in action.

By wiring all assets together, we can write true end-to-end tests by sending commands and asserting the changes in the read store. Have a look at the `GiftcardFeatures` class in the `GiftcardService.Scenarios` project to see how clean and readable such tests can be expressed.
