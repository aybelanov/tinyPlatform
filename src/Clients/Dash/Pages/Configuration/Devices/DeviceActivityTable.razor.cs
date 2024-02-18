using Clients.Dash.Infrastructure;
using Clients.Dash.Models;
using Clients.Dash.Services.EntityServices;
using Microsoft.AspNetCore.Components;
using Shared.Clients;
using System;
using System.Threading.Tasks;
using static Clients.Dash.Pages.Configuration.Users.UserActivityTable;
using Auto = Clients.Dash.Infrastructure.AutoMapper.AutoMapperConfiguration;


namespace Clients.Dash.Pages.Configuration.Devices;

#pragma warning disable CS1591

public partial class DeviceActivityTable
{
   [Inject] ICommonService CommonService { get; set; }

   async Task<IFilterableList<ActivityLogRecordModel>> PrepareActivityLogModelAsync(DynamicFilter filter) 
   {
      var records = await CommonService.GetDeviceActivityLogAsync(filter);
      var models = Auto.Mapper.Map<FilterableList<ActivityLogRecordModel>>(records);
      models.TotalCount = records.TotalCount;

      return models;
   }
}
