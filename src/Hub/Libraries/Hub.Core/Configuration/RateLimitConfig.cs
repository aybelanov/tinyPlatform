using System;
using System.Threading.RateLimiting;

namespace Hub.Core.Configuration;

/// <summary>
/// Represents rate limit configuration (per authorized user user)
/// </summary>
public class RateLimitConfig : IConfig
{
   /// <summary>
   /// Enables rate limiter services
   /// </summary>
   public bool Enabled { get; private set; } = false;

   /// <summary>
   /// SignalR rate limit config
   /// </summary>
   public Config SignalrRateLimit { get; private set; } = new()
   {
      Enabled = false,
      AutoReplenishment = true,
      PermitLimit = 2,
      QueueLimit = 0,
      Window = 2,
      QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
   };

   /// <summary>
   /// Grpc rate limit config
   /// </summary>
   public Config GrpcClientReadRateLimit { get; private set; } = new()
   {
      Enabled = false,
      AutoReplenishment = true,
      PermitLimit = 1,
      QueueLimit = 0,
      Window = 1,
      QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
   };

   /// <summary>
   /// Common rate limiter for guests
   /// </summary>
   public Config GrpcClientModifyRateLimit { get; private set; } = new()
   {
      Enabled = false,
      AutoReplenishment = true,
      PermitLimit = 1,
      QueueLimit = 0,
      Window = 1,
      QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
   };

   /// <summary>
   /// Grpc rate limit config
   /// </summary>
   public Config GrpcDeviceRateLimit { get; private set; } = new()
   {
      Enabled = true,
      AutoReplenishment = true,
      PermitLimit = 3,
      QueueLimit = 3,
      Window = 3,
      QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
   };

   /// <summary>
   /// Ip cam rate limit config
   /// </summary>
   public Config IpcamRateLimit { get; private set; } = new()
   {
      Enabled = true,
      AutoReplenishment = true,
      PermitLimit = 10,
      QueueLimit = 10,
      Window = 10,
      QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
   };

   /// <summary>
   /// html form (login/register, contactus and etc) rate limit config
   /// </summary>
   public Config FormRateLimit { get; private set; } = new()
   {
      Enabled = true,
      AutoReplenishment = true,
      PermitLimit = 1,
      QueueLimit = 0,
      Window = 5,
      QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
   };

   /// <summary>
   /// Rate limit configuration
   /// </summary>
   /// <remarks>
   /// <see href="https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-8.0"/>
   /// <see href="https://github.com/dotnet/AspNetCore.Docs.Samples/blob/main/fundamentals/middleware/rate-limit/WebRateLimitAuth/appsettings.json"/>
   /// </remarks>
   public class Config
   {
      /// <summary>
      /// Is Rate limit middleware enabled in the request pipeline
      /// </summary>
      public bool Enabled { get; set; } = true;

      /// <summary>
      /// Maximum number of permit counters that can be allowed in a window.
      /// Must be set to a value > 0 by the time these options are passed
      /// to the constructor of <see cref="FixedWindowRateLimiter"/>.
      /// </summary>
      public int PermitLimit { get; set; }

      /// <summary>
      /// Specifies the time window that takes in the requests.
      /// Must be set to a value greater than <see cref="TimeSpan.Zero" /> by the time
      /// these options are passed to the constructor of <see cref="FixedWindowRateLimiter"/>.
      /// </summary>
      public int Window { get; set; }

      /// <summary>
      /// Specifies the minimum period between replenishments in secconds.
      /// Must be set to a value greater than <see cref="TimeSpan.Zero" /> by the time 
      /// these options are passed to the constructor of <see cref="TokenBucketRateLimiter"/>.
      /// </summary>
      public int ReplenishmentPeriod { get; set; }

      /// <summary>
      /// Maximum cumulative permit count of queued acquisition requests.
      /// Must be set to a value >= 0 by the time these options are passed 
      /// to the constructor of <see cref="FixedWindowRateLimiter"/>.
      /// </summary>
      public int QueueLimit { get; set; }

      /// <summary>
      /// Determines the behaviour of <see cref="RateLimiter.AcquireAsync"/> when not enough resources can be leased.
      /// </summary>
      /// <value>
      /// <see cref="QueueProcessingOrder.OldestFirst"/> by default.
      /// </value>
      public QueueProcessingOrder QueueProcessingOrder { get; set; } = QueueProcessingOrder.OldestFirst;

      /// <summary>
      /// Segments per window
      /// </summary>
      public int SegmentsPerWindow { get; set; }

      /// <summary>
      /// Maximum number of tokens that can be in the bucket at any time.
      /// Must be set to a value > 0 by the time these options are passed
      /// to the constructor of <see cref="TokenBucketRateLimiter"/>.
      /// </summary>
      public int TokenLimit { get; set; }

      /// <summary>
      /// Maximum number of tokens that can be in the bucket at any time.
      /// Must be set to a value > 0 by the time these options are passed
      /// to the constructor of <see cref="TokenBucketRateLimiter"/>.
      /// </summary>
      public int TokenLimit2 { get; set; }

      /// <summary>
      /// Specifies the maximum number of tokens to restore each replenishment.
      /// Must be set to a value > 0 by the time these options are passed
      /// to the constructor of <see cref="TokenBucketRateLimiter"/>.
      /// </summary>
      public int TokensPerPeriod { get; set; }

      /// <summary>
      /// Specified whether the <see cref="TokenBucketRateLimiter"/> is automatically replenishing tokens or if someone else
      /// will be calling <see cref="TokenBucketRateLimiter.TryReplenish"/> to replenish tokens.
      /// </summary>
      /// <value>
      /// <see langword="true" /> by default.
      /// </value>
      public bool AutoReplenishment { get; set; }
   }
}
