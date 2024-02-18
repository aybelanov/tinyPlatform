using Shared.Common;
using Shared.Devices.Proto;
using System;
using System.Threading.Tasks;

namespace Devices.Dispatcher.Services;

/// <summary>
/// Represents a command service interface
/// </summary>
public interface ICommandService
{
   /// <summary>
   /// Execute a command with params
   /// </summary>
   /// <param name="command">Command string value</param>
   /// <param name="args">Command parameters</param>
   /// <returns></returns>
   Task ExecuteCommand(string command, params string[] args);

   /// <summary>
   /// Execute a command with params
   /// </summary>
   /// <param name="message">Server message with a command from enumertaion</param>
   /// <returns></returns>
   Task ExecuteCommand(ServerMsg message);

   /// <summary>
   /// Command as action
   /// </summary>
   /// <param name="command">Command action</param>
   /// <returns></returns>
   Task ExecuteCommand(Action command);
}