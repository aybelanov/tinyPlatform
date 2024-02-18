namespace Hub.Web.Framework.Models
{
   /// <summary>
   /// Represents base the application entity model
   /// </summary>
   public partial record BaseAppEntityModel : BaseAppModel
   {
      /// <summary>
      /// Gets or sets model identifier
      /// </summary>
      public virtual long Id { get; set; }
   }
}