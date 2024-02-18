using System.Threading.Tasks;

namespace Devices.Dispatcher.Services.Authentication;

/// <summary>
/// Represents a token provider interface
/// </summary>
public interface ITokenProvider
{
   /// <summary>
   /// Gets an access token
   /// </summary>
   /// <returns></returns>
   public Task<string> GetTokenAsync();
}
