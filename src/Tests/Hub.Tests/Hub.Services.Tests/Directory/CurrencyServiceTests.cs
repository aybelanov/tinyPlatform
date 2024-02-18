﻿using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Hub.Core.Domain.Directory;
using Hub.Services.Directory;

namespace Hub.Services.Tests.Directory
{
   [TestFixture]
    public class CurrencyServiceTests : ServiceTest
    {
        private ICurrencyService _currencyService;
        private IExchangeRatePluginManager _exchangeRatePluginManager;

        private Currency _currencyUsd, _currencyRur, _currencyEur;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _currencyService = GetService<ICurrencyService>();
            _exchangeRatePluginManager = GetService<IExchangeRatePluginManager>();

            _currencyEur = await _currencyService.GetCurrencyByCodeAsync("EUR");
            _currencyUsd = await _currencyService.GetCurrencyByCodeAsync("USD");
            _currencyRur = await _currencyService.GetCurrencyByCodeAsync("RUB");
        }

        [Test]
        public async Task CanLoadExchangeRateProviders()
        {
            var providers = await _exchangeRatePluginManager.LoadAllPluginsAsync();
            providers.Should().NotBeNull();
            providers.Any().Should().BeTrue();
        }

        [Test]
        public async Task CanLoadExchangeRateProviderBySystemKeyword()
        {
            var provider = await _exchangeRatePluginManager.LoadPluginBySystemNameAsync("CurrencyExchange.TestProvider");
            provider.Should().NotBeNull();
        }

        [Test]
        public async Task CanLoadActiveExchangeRateProvider()
        {
            var provider = await _exchangeRatePluginManager.LoadPrimaryPluginAsync();
            provider.Should().NotBeNull();
        }

        [Test]
        public void CanConvertCurrency()
        {
            _currencyService.ConvertCurrency(10.1M, 1.5M).Should().Be(15.15M);
            _currencyService.ConvertCurrency(10.1M, 1).Should().Be(10.1M);
            _currencyService.ConvertCurrency(10.1M, 0).Should().Be(0);
            _currencyService.ConvertCurrency(0, 5).Should().Be(0);
        }

        [Test]
        public async Task CanConvertToPrimaryPlatformCurrency()
        {
            var newCurrency = await _currencyService.ConvertToPrimaryPlatformCurrencyAsync(8.686M, _currencyEur);
            newCurrency.Should().Be(10.1M);
            newCurrency = await _currencyService.ConvertToPrimaryPlatformCurrencyAsync(638.825M, _currencyRur);
            newCurrency.Should().Be(10.1M);
            newCurrency = await _currencyService.ConvertToPrimaryPlatformCurrencyAsync(759M, _currencyUsd);
            newCurrency.Should().Be(759M);
        }

        [Test]
        public async Task CanConvertFromPrimaryPlatformCurrency()
        {
            var newCurrency = await _currencyService.ConvertFromPrimaryPlatformCurrencyAsync(10.1M, _currencyEur);
            newCurrency.Should().Be(8.686M);
            newCurrency = await _currencyService.ConvertFromPrimaryPlatformCurrencyAsync(10.1M, _currencyRur);
            newCurrency.Should().Be(638.825M);
            newCurrency = await _currencyService.ConvertFromPrimaryPlatformCurrencyAsync(759M, _currencyUsd);
            newCurrency.Should().Be(759M);
        }

        [Test]
        public async Task TestCrud()
        {
            var insertItem = new Currency
            {
                Name = "Test name",
                CurrencyCode = "tn"
            };

            var updateItem = new Currency
            {
                Name = "Test name 1",
                CurrencyCode = "tn"
            };

            await TestCrud(insertItem, updateItem, (item, other) => item.Name.Equals(other.Name));
        }
    }
}
