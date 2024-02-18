using System.Threading.Tasks;

namespace Hub.Web.Framework.Themes
{
   /// <summary>
   /// Represents a theme context
   /// </summary>
   public interface IThemeContext
   {
      /// <summary>
      /// Get current theme system name
      /// </summary>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task<string> GetWorkingThemeNameAsync();

      /// <summary>
      /// GetTable current theme system name
      /// </summary>
      /// <returns>A task that represents the asynchronous operation</returns>
      Task SetWorkingThemeNameAsync(string workingThemeName);
   }
}
