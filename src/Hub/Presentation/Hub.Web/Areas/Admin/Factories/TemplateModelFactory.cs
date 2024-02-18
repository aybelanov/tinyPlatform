using System;
using System.Linq;
using System.Threading.Tasks;
using Hub.Services.Topics;
using Hub.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Hub.Web.Areas.Admin.Models.Templates;
using Hub.Web.Framework.Models.Extensions;

namespace Hub.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the template model factory implementation
/// </summary>
public partial class TemplateModelFactory : ITemplateModelFactory
{
   #region Fields

   private readonly ITopicTemplateService _topicTemplateService;

   #endregion

   #region Ctor

   public TemplateModelFactory(ITopicTemplateService topicTemplateService)
   {
      _topicTemplateService = topicTemplateService;
   }

   #endregion

   #region Methods

   /// <summary>
   /// Prepare templates model
   /// </summary>
   /// <param name="model">Templates model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the mplates model
   /// </returns>
   public virtual async Task<TemplatesModel> PrepareTemplatesModelAsync(TemplatesModel model)
   {
      if (model == null)
         throw new ArgumentNullException(nameof(model));

      //prepare nested search models
      await PrepareTopicTemplateSearchModelAsync(model.TemplatesTopic);

      return model;
   }

   /// <summary>
   /// Prepare paged topic template list model
   /// </summary>
   /// <param name="searchModel">Topic template search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the opic template list model
   /// </returns>
   public virtual async Task<TopicTemplateListModel> PrepareTopicTemplateListModelAsync(TopicTemplateSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //get topic templates
      var topicTemplates = (await _topicTemplateService.GetAllTopicTemplatesAsync()).ToPagedList(searchModel);

      //prepare grid model
      var model = new TopicTemplateListModel().PrepareToGrid(searchModel, topicTemplates,
          () => topicTemplates.Select(template => template.ToModel<TopicTemplateModel>()));

      return model;
   }

   /// <summary>
   /// Prepare topic template search model
   /// </summary>
   /// <param name="searchModel">Topic template search model</param>
   /// <returns>
   /// A task that represents the asynchronous operation
   /// The task result contains the opic template search model
   /// </returns>
   public virtual Task<TopicTemplateSearchModel> PrepareTopicTemplateSearchModelAsync(TopicTemplateSearchModel searchModel)
   {
      if (searchModel == null)
         throw new ArgumentNullException(nameof(searchModel));

      //prepare page parameters
      searchModel.SetGridPageSize();

      return Task.FromResult(searchModel);
   }

   #endregion
}