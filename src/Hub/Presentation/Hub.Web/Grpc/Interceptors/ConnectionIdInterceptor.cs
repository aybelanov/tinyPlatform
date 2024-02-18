using Grpc.Core;
using Grpc.Core.Interceptors;
using Hub.Core;
using Hub.Core.Configuration;
using Hub.Services.Clients;
using Hub.Services.Users;
using System.Net;
using System.Threading.Tasks;

namespace Hub.Web.Grpc.Interceptors;

public class ConnectionIdInterceptor(IUserService userService, IWorkContext workContext, ICommunicator communicator, AppSettings appSettings) : Interceptor
{
   #region Fields

   private readonly IUserService _userService = userService;
   private readonly IWorkContext _workContext = workContext;
   private readonly ICommunicator _communicator = communicator;
   private readonly SecurityConfig _securityConfig = appSettings.Get<SecurityConfig>();   

   #endregion

   #region Methods

   /// <inheritdoc/>
   public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
      TRequest request,
      ServerCallContext context,
      UnaryServerMethod<TRequest,
      TResponse> continuation)
   {
      await EnsureConnectionId(request, context);
      return await continuation(request, context);
   }

   #endregion

   #region

   /// <summary>
   /// Checks connection identifier
   /// </summary>
   /// <typeparam name="TRequest">Request</typeparam>
   /// <param name="request"></param>
   /// <param name="context"></param>
   /// <returns></returns>
   private async Task EnsureConnectionId<TRequest>(TRequest request, ServerCallContext context)
   {
      if (!_securityConfig.RequireSignalrConnection)
         return;

      var httpContext = context.GetHttpContext();
      if (httpContext.User.Identity?.IsAuthenticated ?? false)
      {
         var connetionId = await _workContext.GetCurrentConncetionIdAsync();
         if (!string.IsNullOrEmpty(connetionId))
         {
            var connectionInfo = await _communicator.GetConnectionInfoByConnectionIdAsync(connetionId);
            if (connectionInfo != null)
            {
               var user = await _userService.GetUserByIdAsync(connectionInfo.UserId);
               if (user != null && user.IsActive && !user.IsDeleted && !user.RequireReLogin)
               {
                  return;
               }
            }
         }
      }

      throw new RpcException(new Status(StatusCode.PermissionDenied, "Conection id is not presented."));
   }

   #endregion
}
