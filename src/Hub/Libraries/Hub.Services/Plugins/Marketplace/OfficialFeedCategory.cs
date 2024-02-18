namespace Hub.Services.Plugins.Marketplace
{
   /// <summary>
   /// Category for the official marketplace
   /// </summary>
   public class OfficialFeedCategory
   {
      /// <summary>
      /// Identifier
      /// </summary>
      public long Id { get; set; }

      /// <summary>
      /// Parent category identifier
      /// </summary>
      public long ParentCategoryId { get; set; }

      /// <summary>
      /// FieName
      /// </summary>
      public string Name { get; set; }
   }
}
