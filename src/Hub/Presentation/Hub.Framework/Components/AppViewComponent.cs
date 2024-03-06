using Hub.Core.Events;
using Hub.Core.Infrastructure;
using Hub.Web.Framework.Events;
using Hub.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Collections.Generic;

namespace Hub.Web.Framework.Components;

/// <summary>
/// Base class for ViewComponent in the app
/// </summary>
public abstract class AppViewComponent : ViewComponent
{
   private void PublishModelPrepared<TModel>(TModel model)
   {
      //Components are not part of the controller life cycle.
      //Hence, we could no longer use Action Filters to intercept the Models being returned
      //as we do in the /App.Web.Framework/Mvc/Filters/PublishModelEventsAttribute.cs for controllers

      //model prepared event
      if (model is BaseAppModel)
      {
         var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();

         //we publish the ModelPrepared event for all models as the BaseAppModel, 
         //so you need to implement IConsumer<ModelPrepared<BaseAppModel>> interface to handle this event
         eventPublisher.ModelPreparedAsync(model as BaseAppModel).Wait();
      }

      if (model is IEnumerable<BaseAppModel> modelCollection)
      {
         var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();

         //we publish the ModelPrepared event for collection as the IEnumerable<BaseAppModel>, 
         //so you need to implement IConsumer<ModelPrepared<IEnumerable<BaseAppModel>>> interface to handle this event
         eventPublisher.ModelPreparedAsync(modelCollection).Wait();
      }
   }
   /// <summary>
   /// Returns a result which will render the partial view with name <paramref name="viewName"/>.
   /// </summary>
   /// <param name="viewName">The name of the partial view to render.</param>
   /// <param name="model">The model object for the view.</param>
   /// <returns>A <see cref="ViewViewComponentResult"/>.</returns>
   public new ViewViewComponentResult View<TModel>(string viewName, TModel model)
   {
      PublishModelPrepared(model);

      //invoke the base method
      return base.View(viewName, model);
   }

   /// <summary>
   /// Returns a result which will render the partial view
   /// </summary>
   /// <param name="model">The model object for the view.</param>
   /// <returns>A <see cref="ViewViewComponentResult"/>.</returns>
   public new ViewViewComponentResult View<TModel>(TModel model)
   {
      PublishModelPrepared(model);

      //invoke the base method
      return base.View(model);
   }

   /// <summary>
   ///  Returns a result which will render the partial view with name viewName
   /// </summary>
   /// <param name="viewName">The name of the partial view to render.</param>
   /// <returns>A <see cref="ViewViewComponentResult"/>.</returns>
   public new ViewViewComponentResult View(string viewName)
   {
      //invoke the base method
      return base.View(viewName);
   }
}