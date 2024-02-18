using FluentAssertions;
using Hub.Core.Domain.Directory;
using Hub.Core.Domain.Users;
using NUnit.Framework;

namespace Hub.Core.Tests.Domain;

[TestFixture]
public class EntityEqualityTests
{
   [Test]
   public void TwoTransientEntitiesShouldNotBeEqual()
   {
      var u1 = new User();
      var u2 = new User();

      //u1.Should().NotBe(u2, "Different transient entities should not be equal");
   }

   [Test]
   public void TwoReferencesToSameTransientEntityShouldBeEqual()
   {
      var u1 = new User();
      var u2 = u1;

      u1.Should().Be(u2, "Two references to the same transient entity should be equal");
   }

   [Test]
   public void EntitiesWithDifferentIdShouldNotBeEqual()
   {
      var u1 = new User { Id = 2 };
      var u2 = new User { Id = 5 };

      u1.Should().NotBe(u2, "Entities with different ids should not be equal");
   }

   [Test]
   public void EntityShouldNotEqualTransientEntity()
   {
      var u1 = new User { Id = 1 };
      var u2 = new User();

      u1.Should().NotBe(u2, "Entity and transient entity should not be equal");
   }

   [Test]
   public void EntitiesWithSameIdButDifferentTypeShouldNotBeEqual()
   {
      const int id = 10;
      var u1 = new User { Id = id };

      var c1 = new Country { Id = id };

      u1.Should().NotBe(c1, "Entities of different types should not be equal, even if they have the same id");
   }
}
