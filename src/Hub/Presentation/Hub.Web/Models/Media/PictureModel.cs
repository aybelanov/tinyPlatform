using Hub.Web.Framework.Models;

namespace Hub.Web.Models.Media
{
   public partial record PictureModel : BaseAppModel
   {
      public string ImageUrl { get; set; }

      public string ThumbImageUrl { get; set; }

      public string FullSizeImageUrl { get; set; }

      public string Title { get; set; }

      public string AlternateText { get; set; }
   }
}