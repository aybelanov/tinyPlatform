using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Hub.Core;
using Hub.Tests;

namespace Hub.Core.Tests;

[TestFixture]
public class WebHelperTests : BaseAppTest
{
   private HttpContext _httpContext;
   private IWebHelper _webHelper;

   [OneTimeSetUp]
   public void SetUp()
   {
      _webHelper = GetService<IWebHelper>();
      _httpContext = GetService<IHttpContextAccessor>().HttpContext;

      var queryString = new QueryString(string.Empty);
      queryString = queryString.Add("Key1", "Value1");
      queryString = queryString.Add("Key2", "Value2");
      _httpContext.Request.QueryString = queryString;
   }

   [OneTimeTearDown]
   public void TearDown()
   {
      var queryString = new QueryString(string.Empty);
      _httpContext.Request.QueryString = queryString;
   }

   [Test]
   public void CanGetAppHostWithoutSsl()
   {
      _webHelper.GetPlatformHost(false).Should().Be($"http://{AppTestsDefaults.HostIpAddress}/");
   }

   [Test]
   public void CanGetAppHostWithSsl()
   {
      _webHelper.GetPlatformHost(true).Should().Be($"https://{AppTestsDefaults.HostIpAddress}/");
   }

   [Test]
   public void CanGetAppLocationWithoutSsl()
   {
      _webHelper.GetAppLocation(false).Should().Be($"http://{AppTestsDefaults.HostIpAddress}/");
   }

   [Test]
   public void CanGetAppLocationWithSsl()
   {
      _webHelper.GetAppLocation(true).Should().Be($"https://{AppTestsDefaults.HostIpAddress}/");
   }

   [Test]
   public void CanGetAppLocationInVirtualDirectory()
   {
      _httpContext.Request.PathBase = "/applicationpath";
      _webHelper.GetAppLocation(false).Should().Be($"http://{AppTestsDefaults.HostIpAddress}/applicationpath/");
      _httpContext.Request.PathBase = string.Empty;
   }

   [Test]
   public void CanGetQueryString()
   {
      _webHelper.QueryString<string>("Key1").Should().Be("Value1");
      _webHelper.QueryString<string>("Key2").Should().Be("Value2");
      _webHelper.QueryString<string>("Key3").Should().Be(null);
   }

   [Test]
   public void CanRemoveQueryString()
   {
      //empty URL
      _webHelper.RemoveQueryString(null, null).Should().Be(string.Empty);
      //empty key
      _webHelper.RemoveQueryString($"http://{AppTestsDefaults.HostIpAddress}/", null).Should().Be($"http://{AppTestsDefaults.HostIpAddress}/");
      //non-existing param with fragment
      _webHelper.RemoveQueryString($"http://{AppTestsDefaults.HostIpAddress}/#fragment", "param").Should().Be($"http://{AppTestsDefaults.HostIpAddress}/#fragment");
      //first param (?)
      _webHelper.RemoveQueryString($"http://{AppTestsDefaults.HostIpAddress}/?param1=value1&param2=value1", "param1")
          .Should().Be($"http://{AppTestsDefaults.HostIpAddress}/?param2=value1");
      //second param (&)
      _webHelper.RemoveQueryString($"http://{AppTestsDefaults.HostIpAddress}/?param1=value1&param2=value1", "param2")
          .Should().Be($"http://{AppTestsDefaults.HostIpAddress}/?param1=value1");
      //non-existing param
      _webHelper.RemoveQueryString($"http://{AppTestsDefaults.HostIpAddress}/?param1=value1&param2=value1", "param3")
          .Should().Be($"http://{AppTestsDefaults.HostIpAddress}/?param1=value1&param2=value1");
      //with fragment
      _webHelper.RemoveQueryString($"http://{AppTestsDefaults.HostIpAddress}/?param1=value1&param2=value1#fragment", "param1")
          .Should().Be($"http://{AppTestsDefaults.HostIpAddress}/?param2=value1#fragment");
      //specific value
      _webHelper.RemoveQueryString($"http://{AppTestsDefaults.HostIpAddress}/?param1=value1&param1=value2&param2=value1", "param1", "value1")
          .Should().Be($"http://{AppTestsDefaults.HostIpAddress}/?param1=value2&param2=value1");
      //all values
      _webHelper.RemoveQueryString($"http://{AppTestsDefaults.HostIpAddress}/?param1=value1&param1=value2&param2=value1", "param1")
          .Should().Be($"http://{AppTestsDefaults.HostIpAddress}/?param2=value1");
   }

   [Test]
   public void CanModifyQueryString()
   {
      //empty URL
      _webHelper.ModifyQueryString(null, null).Should().Be(string.Empty);
      //empty key
      _webHelper.ModifyQueryString($"http://{AppTestsDefaults.HostIpAddress}/", null).Should().Be($"http://{AppTestsDefaults.HostIpAddress}/");
      //empty value
      _webHelper.ModifyQueryString($"http://{AppTestsDefaults.HostIpAddress}/", "param").Should().Be($"http://{AppTestsDefaults.HostIpAddress}/?param=");
      //first param (?)
      _webHelper.ModifyQueryString($"http://{AppTestsDefaults.HostIpAddress}/?param1=value1&param2=value1", "Param1", "value2")
          .Should().Be($"http://{AppTestsDefaults.HostIpAddress}/?param1=value2&param2=value1");
      //second param (&)
      _webHelper.ModifyQueryString($"http://{AppTestsDefaults.HostIpAddress}/?param1=value1&param2=value1", "param2", "value2")
          .Should().Be($"http://{AppTestsDefaults.HostIpAddress}/?param1=value1&param2=value2");
      //non-existing param
      _webHelper.ModifyQueryString($"http://{AppTestsDefaults.HostIpAddress}/?param1=value1&param2=value1", "param3", "value1")
          .Should().Be($"http://{AppTestsDefaults.HostIpAddress}/?param1=value1&param2=value1&param3=value1");
      //multiple values
      _webHelper.ModifyQueryString($"http://{AppTestsDefaults.HostIpAddress}/?param1=value1&param2=value1", "param1", "value1", "value2", "value3")
          .Should().Be($"http://{AppTestsDefaults.HostIpAddress}/?param1=value1,value2,value3&param2=value1");
      //with fragment
      _webHelper.ModifyQueryString($"http://{AppTestsDefaults.HostIpAddress}/?param1=value1&param2=value1#fragment", "param1", "value2")
          .Should().Be($"http://{AppTestsDefaults.HostIpAddress}/?param1=value2&param2=value1#fragment");
   }

   [Test]
   public void CanModifyQueryStringInVirtualDirectory()
   {
      _httpContext.Request.PathBase = "/applicationpath";
      _webHelper.ModifyQueryString("/applicationpath/Controller/Action", "param1", "value1").Should().Be("/applicationpath/Controller/Action?param1=value1");
      _httpContext.Request.PathBase = string.Empty;
   }
}
