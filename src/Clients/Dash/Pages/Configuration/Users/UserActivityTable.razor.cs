using Clients.Dash.Models;
using Clients.Dash.Services.EntityServices;
using Microsoft.AspNetCore.Components;
using Shared.Clients;
using System;
using System.Threading.Tasks;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;

namespace Clients.Dash.Pages.Configuration.Users;

#pragma warning disable CS1591

public partial class UserActivityTable
{
   [Inject] ICommonService CommonService { get; set; }

   async Task<IFilterableList<ActivityLogRecordModel>> PrepareActivityLogModelAsync(DynamicFilter filter)
   {
      var records = await CommonService.GetUserActivityLogAsync(filter);
      var models = Auto.Mapper.Map<FilterableList<ActivityLogRecordModel>>(records);
      models.TotalCount = records.TotalCount;

      return models;
   }

   /// <summary>
   /// Represents activity log record
   /// </summary>
   public class ActivityLogRecordModel : BaseEntityModel
   {
      /// <summary>
      /// Gets or sets the activity log type identifier
      /// </summary>
      public string ActivityType { get; set; }

      /// <summary>
      /// Gets or sets the entity name
      /// </summary>
      public string EntityName { get; set; }

      /// <summary>
      /// Gets or sets the activity comment
      /// </summary>
      public string Comment { get; set; }

      /// <summary>
      /// Gets or sets the date and time of instance creation
      /// </summary>
      public DateTime CreatedOnUtc { get; set; }

      /// <summary>
      /// Gets or sets the IP address
      /// </summary>
      public virtual string IpAddress { get; set; }
   }
}

#pragma warning restore CS1591