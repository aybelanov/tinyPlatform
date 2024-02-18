using Hub.Core.Domain.Directory;
using Hub.Core.Domain.Users;
using Shared.Common;
using System;
using System.Collections.Generic;

namespace Hub.Core.Domain.Common;

/// <summary>
/// Address
/// </summary>
public partial class Address : BaseEntity
{
   /// <summary>
   /// Gets or sets the first name
   /// </summary>
   public string FirstName { get; set; }

   /// <summary>
   /// Gets or sets the last name
   /// </summary>
   public string LastName { get; set; }

   /// <summary>
   /// Gets or sets the email
   /// </summary>
   public string Email { get; set; }

   /// <summary>
   /// Gets or sets the company
   /// </summary>
   public string Company { get; set; }

   /// <summary>
   /// Gets or sets the country identifier
   /// </summary>
   public long? CountryId { get; set; }

   /// <summary>
   /// Gets or sets the state/province identifier
   /// </summary>
   public long? StateProvinceId { get; set; }

   /// <summary>
   /// Gets or sets the county
   /// </summary>
   public string County { get; set; }

   /// <summary>
   /// Gets or sets the city
   /// </summary>
   public string City { get; set; }

   /// <summary>
   /// Gets or sets the address 1
   /// </summary>
   public string Address1 { get; set; }

   /// <summary>
   /// Gets or sets the address 2
   /// </summary>
   public string Address2 { get; set; }

   /// <summary>
   /// Gets or sets the zip/postal code
   /// </summary>
   public string ZipPostalCode { get; set; }

   /// <summary>
   /// Gets or sets the phone number
   /// </summary>
   public string PhoneNumber { get; set; }

   /// <summary>
   /// Gets or sets the fax number
   /// </summary>
   public string FaxNumber { get; set; }

   /// <summary>
   /// Gets or sets the custom attributes (see "AddressAttribute" entity for more info)
   /// </summary>
   public string CustomAttributes { get; set; }

   /// <summary>
   /// Gets or sets the date and time of instance creation
   /// </summary>
   public DateTime CreatedOnUtc { get; set; }

//   #region Navigation
//#pragma warning disable CS1591

//   public Country Country { get; set; }
   
//   public StateProvince StateProvince { get; set; }
   
//   public List<Affiliate> Affiliates { get; set; } = new();

//   public List<User> Users { get; set; } = new();

//   public List<UserAddress> UsersAddresses { get; set; } = new();  

//#pragma warning restore CS1591
//   #endregion
}
