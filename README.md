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