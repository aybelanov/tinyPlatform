using System.Collections.Generic;
using System.Linq;

namespace Hub.Services.Devices;

/// <summary>
/// Device registration result
/// </summary>
public class DeviceRegistrationResult
{
   /// <summary> Default Ctor </summary>
   public DeviceRegistrationResult()
   {
      Errors = new List<string>();
   }

   /// <summary>
   /// Gets a value indicating whether request has been completed successfully
   /// </summary>
   public bool Success => !Errors.Any();

   /// <summary>
   /// Add error
   /// </summary>
   /// <param name="error">Error</param>
   public void AddError(string error)
   {
      Errors.Add(error);
   }

   /// <summary>
   /// Errors
   /// </summary>
   public IList<string> Errors { get; set; }
}