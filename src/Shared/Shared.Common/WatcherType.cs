namespace Shared.Common;

/// <summary>
/// Watcher type
/// </summary>
public enum WatcherType
{
#pragma warning disable CS1591

   RS485 = 1,
   CANBus,
   IPCam,
   ADC,
   System,
   RS232,
   Program,
   Emulator

#pragma warning restore CS1591
}