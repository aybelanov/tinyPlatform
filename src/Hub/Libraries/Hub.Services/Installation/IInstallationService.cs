using System.Globalization;
using System.Threading.Tasks;

namespace Hub.Services.Installation;

/// <summary>
/// Installation service
/// </summary>
public partial interface IInstallationService
{
   /// <summary>
   /// Install required data
   /// </summary>
   /// <param name="defaultUserEmail">Default user email</param>
   /// <param name="defaultUserPassword">Default user password</param>
   /// <param name="regionInfo">RegionInfo</param>
   /// <param name="cultureInfo">CultureInfo</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task InstallRequiredDataAsync(string defaultUserEmail, string defaultUserPassword, RegionInfo regionInfo, CultureInfo cultureInfo);

   /// <summary>
   /// Install sample data
   /// </summary>
   /// <param name="defaultUserEmail">Default user email</param>
   /// <returns>A task that represents the asynchronous operation</returns>
   Task InstallSampleDataAsync(string defaultUserEmail);
}