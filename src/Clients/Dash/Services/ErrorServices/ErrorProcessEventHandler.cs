namespace Clients.Dash.Services.ErrorServices;

/// <summary>
/// Error process handler for an erroro event 
/// </summary>
/// <param name="sender">Error sender</param>
/// <param name="e">Error event arguments</param>
public delegate void ErrorProcessEventHandler(object sender, ErrorEventArgs e);
