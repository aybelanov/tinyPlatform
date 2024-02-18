using Clients.Dash.Domain;
using Clients.Dash.Models;
using System.Collections.Generic;
using Monitor = Clients.Dash.Domain.Monitor;

namespace Clients.Dash.Pages.Monitors;

/// <summary>
/// Represents a monitor presentstion view model class
/// </summary>
public class MonitorViewModel : BaseEntityModel
{
   /// <summary>
   /// Monitor name
   /// </summary>
   public string Name { get; set; }

   /// <summary>
   /// Monitor description
   /// </summary>
   public string Description { get; set; }

   /// <summary>
   /// Device user-owner name
   /// </summary>
   public string OwnerName { get; set; }

   /// <summary>
   /// Presentation collection
   /// </summary>
   public List<PresentationViewModel> Presentations { get; set; }
}