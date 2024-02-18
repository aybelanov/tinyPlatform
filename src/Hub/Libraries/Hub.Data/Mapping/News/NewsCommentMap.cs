using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub.Core.Domain.News;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Data.Mapping.News;

/// <summary>
/// Mapping class
/// </summary>
public class NewsCommentMap : AppEntityTypeConfiguration<NewsComment>
{
   /// <summary>
   /// Configures a mapping
   /// </summary>
   /// <param name="builder">Entity type builder</param>
   public override void Configure(EntityTypeBuilder<NewsComment> builder)
   {
      builder.ToTable("NewsComments");

      base.Configure(builder);
   }
}
