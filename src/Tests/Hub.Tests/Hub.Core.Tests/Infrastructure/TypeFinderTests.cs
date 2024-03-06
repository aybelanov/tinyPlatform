using FluentAssertions;
using Hub.Core.Infrastructure;
using Hub.Tests;
using NUnit.Framework;
using System.Linq;

namespace Hub.Core.Tests.Infrastructure;

[TestFixture]
public class TypeFinderTests : BaseAppTest
{
   [Test]
   public void TypeFinderBenchmarkFindings()
   {
      var finder = GetService<ITypeFinder>();
      var type = finder.FindClassesOfType<ISomeInterface>().ToList();
      type.Count.Should().Be(1);
      typeof(ISomeInterface).IsAssignableFrom(type.FirstOrDefault()).Should().BeTrue();
   }

   public interface ISomeInterface
   {
   }

   public class SomeClass : ISomeInterface
   {
   }
}
