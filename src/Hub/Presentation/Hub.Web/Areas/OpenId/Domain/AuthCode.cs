using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Web.Areas.OpenId.Domain;

/// <summary>
/// Authentication code 
/// </summary>
public class AuthCode
{
   /// <summary>
   /// Client identifier
   /// </summary>
   public string ClientId { get; set; }

   /// <summary>
   /// Subject identifier
   /// </summary>
   public string SubjectId { get; set; }

   /// <summary>
   /// Code challenge
   /// </summary>
   public string CodeChallenge { get; set; }
   /// <summary>
   /// Code challenge method
   /// </summary>
   public string CodeChallengeMethod { get; set; }

   /// <summary>
   /// Reddirect uri
   /// </summary>
   public string RedirectUri { get; set; }

   /// <summary>
   /// Code expires in
   /// </summary>
   public DateTime Expired { get; set; }
}
