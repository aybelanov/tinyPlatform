using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Hub.Core.ComponentModel;
using Hub.Core.Infrastructure;

namespace Hub.Core
{
   /// <summary>
   /// Startup task for the registration custom type converters
   /// </summary>
   public class TypeConverterRegistrationStartUpTask : IStartupTask
   {
      /// <summary>
      /// Executes a task
      /// </summary>
      /// <returns>A task that represents the asynchronous operation</returns>
      public Task ExecuteAsync()
      {
         //lists
         TypeDescriptor.AddAttributes(typeof(List<int>), new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
         TypeDescriptor.AddAttributes(typeof(List<decimal>), new TypeConverterAttribute(typeof(GenericListTypeConverter<decimal>)));
         TypeDescriptor.AddAttributes(typeof(List<string>), new TypeConverterAttribute(typeof(GenericListTypeConverter<string>)));

         //dictionaries
         TypeDescriptor.AddAttributes(typeof(Dictionary<int, int>), new TypeConverterAttribute(typeof(GenericDictionaryTypeConverter<int, int>)));

         return Task.CompletedTask;
      }

      /// <summary>
      /// Gets order of this startup task implementation
      /// </summary>
      public int Order => 1;
   }
}
