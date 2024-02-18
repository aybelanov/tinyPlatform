using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Hub.Core;
using Hub.Core.Domain.Common;
using Hub.Core.Domain.Users;
using Hub.Services.Common;
using Hub.Services.Directory;
using Hub.Tests;
using Hub.Web.Factories;
using Hub.Web.Models.Common;
using NUnit.Framework;

namespace Hub.Web.Tests.Public.Factories
{
    [TestFixture]
    public class AddressModelFactoryTests: BaseAppTest
    {
        private IAddressModelFactory _addressModelFactory;
        private IGenericAttributeService _genericAttributeService;
        private Address _address;
        private AddressSettings _addressSettings;
        private ICountryService _countryService;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _addressModelFactory = GetService<IAddressModelFactory>();
            _genericAttributeService = GetService<IGenericAttributeService>();

            _address = await GetService<IAddressService>().GetAddressByIdAsync(1);
            _addressSettings = GetService<AddressSettings>();
            _countryService = GetService<ICountryService>();
        }

        [Test]
        public async Task PrepareAddressModelShouldPopulatingPropertiesFromEntity()
        {
            var model = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(model, _address, false, _addressSettings);

            model.Id.Should().Be(_address.Id);
            model.FirstName.Should().Be(_address.FirstName);
            model.LastName.Should().Be(_address.LastName);
            model.Email.Should().Be(_address.Email);
            model.Company.Should().Be(_address.Company);
            model.CountryId.Should().Be(_address.CountryId);
            model.StateProvinceId.Should().Be(_address.StateProvinceId);
            model.County.Should().Be(_address.County);
            model.City.Should().Be(_address.City);
            model.Address1.Should().Be(_address.Address1);
            model.Address2.Should().Be(_address.Address2);
            model.ZipPostalCode.Should().Be(_address.ZipPostalCode);
            model.PhoneNumber.Should().Be(_address.PhoneNumber);
            model.FaxNumber.Should().Be(_address.FaxNumber);
        }

        [Test]
        public async Task PrepareAddressModelShouldNotPopulatingPropertiesFromEntityIfExcludePropertiesFlagEnabled()
        {
            var model = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(model, _address, true, _addressSettings);

            model.Id.Should().Be(0);
            model.FirstName.Should().BeNull();
            model.LastName.Should().BeNull();
            model.Email.Should().BeNull();
            model.Company.Should().BeNull();
            model.CountryId.Should().BeNull();
            model.StateProvinceId.Should().BeNull();
            model.County.Should().BeNull();
            model.City.Should().BeNull();
            model.Address1.Should().BeNull();
            model.Address2.Should().BeNull();
            model.ZipPostalCode.Should().BeNull();
            model.PhoneNumber.Should().BeNull();
            model.FaxNumber.Should().BeNull();
        }

        [Test]
        public async Task PrepareAddressModelShouldNotPopulatingPropertiesFromEntityIfEntityIsNull()
        {
            var model = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(model, null, false, _addressSettings);

            model.Id.Should().Be(0);
            model.FirstName.Should().BeNull();
            model.LastName.Should().BeNull();
            model.Email.Should().BeNull();
            model.Company.Should().BeNull();
            model.CountryId.Should().BeNull();
            model.StateProvinceId.Should().BeNull();
            model.County.Should().BeNull();
            model.City.Should().BeNull();
            model.Address1.Should().BeNull();
            model.Address2.Should().BeNull();
            model.ZipPostalCode.Should().BeNull();
            model.PhoneNumber.Should().BeNull();
            model.FaxNumber.Should().BeNull();
        }

        [Test]
        public void PrepareAddressModelShouldRaiseExceptionIfPrePopulateWithUserFieldsFlagEnabledButUserNotPassed()
        {
            var model = new AddressModel();

            Assert.Throws<AggregateException>(() =>
                _addressModelFactory.PrepareAddressModelAsync(model, null, false, _addressSettings,
                    prePopulateWithUserFields: true).Wait());
        }

        [Test]
        public async Task PrepareAddressModelShouldFillAvailableCountriesAndAvailableStates()
        {
            var model = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(model, null, false, _addressSettings,
                async () => await _countryService.GetAllCountriesAsync());
            model.AvailableCountries.Any().Should().BeTrue();
            model.AvailableCountries.Count.Should().Be(250);
            model.AvailableStates.Any().Should().BeTrue();
            model.AvailableStates.Count.Should().Be(1);

            model = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(model, _address, false, _addressSettings,
                async () => await _countryService.GetAllCountriesAsync());
            model.AvailableCountries.Any().Should().BeTrue();
            model.AvailableCountries.Count.Should().Be(250);
            model.AvailableStates.Any().Should().BeTrue();
            model.AvailableStates.Count.Should().Be(63);
        }

        [Test]
        public async Task PrepareAddressModelShouldFillUsersInfoIfPrePopulateWithUserFieldsFlagEnabledAndUserPassed()
        {
            var model = new AddressModel();
            var user = await GetService<IWorkContext>().GetCurrentUserAsync();
            await _addressModelFactory.PrepareAddressModelAsync(model, null, false, _addressSettings,
                prePopulateWithUserFields: true, user: user);

            model.Email.Should().Be(user.Email);
            model.FirstName.Should().Be(await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.FirstNameAttribute));
            model.LastName.Should().Be(await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.LastNameAttribute));

            model.Company.Should().Be(await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.CompanyAttribute));
            model.Address1.Should().Be(await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.StreetAddressAttribute));
            model.Address2.Should().Be(await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.StreetAddress2Attribute));
            model.ZipPostalCode.Should().Be(await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.ZipPostalCodeAttribute));
            model.City.Should().Be(await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.CityAttribute));
            model.County.Should().Be(await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.CountyAttribute));
            model.PhoneNumber.Should().Be(await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.PhoneAttribute));
            model.FaxNumber.Should().Be(await _genericAttributeService.GetAttributeAsync<string>(user, AppUserDefaults.FaxAttribute));
        }
    }
}
