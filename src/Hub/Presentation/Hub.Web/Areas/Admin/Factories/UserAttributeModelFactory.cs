using System;
using System.Linq;
using System.Threading.Tasks;
using Hub.Web.Areas.Admin.Models.Users;
using Hub.Core.Domain.Users;
using Hub.Services.Users;
using Hub.Services.Localization;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Framework.Factories;
using Hub.Web.Framework.Models.Extensions;
using Hub.Data.Extensions;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the user attribute model factory implementation
/// </summary>
public partial class UserAttributeModelFactory : IUserAttributeModelFactory
{
   #region Fields

   private readonly IUserAttributeService _userAttributeService;
   private readonly ILocalizationService _localizationService;
   private readonly ILocalizedModelFactory _localizedModelFactory;

   #endregion

   #region Ctor

   public UserAttributeModelFactory(IUserAttributeService userAttributeService,
       ILocalizationService localizationService,
       ILocalizedModelFactory localizedModelFactory)
   {
      _userAttributeService = userAttributeService;
      _localizationService = localizationService;
      _localizedModelFactory = localizedModelFactory;
   }

   #endregion

   #region Utilities

   /// <summary>
   /// Prepare user attribute value search model
   /// </summary>
   /// <param name="searchModel">User attribute value search model</param>
   /// <param name="userAttribute">User attribute</param>
   /// <returns>User attribute value search model</returns>
   protected virtual UserAttributeValueSearchModel PrepareUserAttributeValueSearchModel(UserAttributeValueSearchModel searchModel,
       UserAttribute userAttribute)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (userAttribute == null)
         throw new ArgumentNullException(nameof(userAttribute));

      searchModel.UserAttributeId = userAttribute.Id;

      //prepare page parameters
      searchModel.SetGridPageSize();

      return searchModel;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare user attribute search model
   /// </summary>
   /// <param name="searchModel">User attribute search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user attribute search model
   /// </returns>
   public virtual Task<UserAttributeSearchModel> PrepareUserAttributeSearchModelAsync(UserAttributeSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare page parameters
      searchModel.SetGridPageSize();

      return Task.FromResult(searchModel);
   }

   /// <summary>
   /// Prepare paged user attribute list model
   /// </summary>
   /// <param name="searchModel">User attribute search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user attribute list model
   /// </returns>
   public virtual async Task<UserAttributeListModel> PrepareUserAttributeListModelAsync(UserAttributeSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //get user attributes
      var userAttributes = (await _userAttributeService.GetAllUserAttributesAsync()).ToPagedList(searchModel);

      //prepare list model
      var model = await new UserAttributeListModel().PrepareToGridAsync(searchModel, userAttributes, () =>
      {
         return userAttributes.SelectAwait(async attribute =>
            {
               //fill in model values from the entity
               var attributeModel = attribute.ToModel<UserAttributeModel>();

               //fill in additional values (not existing in the entity)
               attributeModel.AttributeControlTypeName = await _localizationService.GetLocalizedEnumAsync(attribute.AttributeControlType);

               return attributeModel;
            });
      });

      return model;
   }

   /// <summary>
   /// Prepare user attribute model
   /// </summary>
   /// <param name="model">User attribute model</param>
   /// <param name="userAttribute">User attribute</param>
   /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user attribute model
   /// </returns>
   public virtual async Task<UserAttributeModel> PrepareUserAttributeModelAsync(UserAttributeModel model,
       UserAttribute userAttribute, bool excludeProperties = false)
   {
      Func<UserAttributeLocalizedModel, long, Task> localizedModelConfiguration = null;

      if (userAttribute != null)
      {
         //fill in model values from the entity
         model ??= userAttribute.ToModel<UserAttributeModel>();

         //prepare nested search model
         PrepareUserAttributeValueSearchModel(model.UserAttributeValueSearchModel, userAttribute);

         //define localized model configuration action
         localizedModelConfiguration = async (locale, languageId) =>
         {
            locale.Name = await _localizationService.GetLocalizedAsync(userAttribute, entity => entity.Name, languageId, false, false);
         };
      }

      //prepare localized models
      if (!excludeProperties)
         model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

      return model;
   }

   /// <summary>
   /// Prepare paged user attribute value list model
   /// </summary>
   /// <param name="searchModel">User attribute value search model</param>
   /// <param name="userAttribute">User attribute</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user attribute value list model
   /// </returns>
   public virtual async Task<UserAttributeValueListModel> PrepareUserAttributeValueListModelAsync(UserAttributeValueSearchModel searchModel,
       UserAttribute userAttribute)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      if (userAttribute == null)
         throw new ArgumentNullException(nameof(userAttribute));

      //get user attribute values
      var userAttributeValues = (await _userAttributeService
          .GetUserAttributeValuesAsync(userAttribute.Id)).ToPagedList(searchModel);

      //prepare list model
      var model = new UserAttributeValueListModel().PrepareToGrid(searchModel, userAttributeValues, () =>
      {
         //fill in model values from the entity
         return userAttributeValues.Select(value => value.ToModel<UserAttributeValueModel>());
      });

      return model;
   }

   /// <summary>
   /// Prepare user attribute value model
   /// </summary>
   /// <param name="model">User attribute value model</param>
   /// <param name="userAttribute">User attribute</param>
   /// <param name="userAttributeValue">User attribute value</param>
   /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the user attribute value model
   /// </returns>
   public virtual async Task<UserAttributeValueModel> PrepareUserAttributeValueModelAsync(UserAttributeValueModel model,
       UserAttribute userAttribute, UserAttributeValue userAttributeValue, bool excludeProperties = false)
   {
      if (userAttribute == null)
         throw new ArgumentNullException(nameof(userAttribute));

      Func<UserAttributeValueLocalizedModel, long, Task> localizedModelConfiguration = null;

      if (userAttributeValue != null)
      {
         //fill in model values from the entity
         model ??= userAttributeValue.ToModel<UserAttributeValueModel>();

         //define localized model configuration action
         localizedModelConfiguration = async (locale, languageId) =>
         {
            locale.Name = await _localizationService.GetLocalizedAsync(userAttributeValue, entity => entity.Name, languageId, false, false);
         };
      }

      model.UserAttributeId = userAttribute.Id;

      //prepare localized models
      if (!excludeProperties)
         model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

      return model;
   }

   #endregion
}