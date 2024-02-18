using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Hub.Web.Areas.Admin.Models.Media
{
    public partial record RoxyFilemanUploadModel(string Action, string Method, string D);
}