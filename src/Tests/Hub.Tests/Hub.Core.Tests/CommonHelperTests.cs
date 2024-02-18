using FluentAssertions;
using NUnit.Framework;
using Hub.Core;

namespace Hub.Core.Tests;

[TestFixture]
public class CommonHelperTests
{
   [Test]
   public void CanGetTypedValue()
   {
      CommonHelper.To<int>("1000").Should().BeOfType(typeof(int));
      CommonHelper.To<int>("1000").Should().Be(1000);
   }
}
