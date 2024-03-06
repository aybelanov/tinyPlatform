using Hub.Web.Framework.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Framework.Controllers;

/// <summary>
/// Represents a base webapi controller 
/// </summary>
[ApiController]
[Route("webapi/[controller]/[action]")]
[EnableCors(PolicyName = WebFrameworkDefaults.CorsPolicyName)]
[Authorize(AuthenticationSchemes = "Bearer")]
public abstract class BaseWebapiController : ControllerBase
{

}