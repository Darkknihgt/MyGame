using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CounterApp;

namespace FrameworkDesign
{
    public interface ICanSendCommand :IBelongToArchitect
    {
        
    }

    public static class CanSendCommandExtension
    {
        public static void SendCommand<T>(this ICanSendCommand self) where T : IAddCountCommand, new()
        {
            self.GetArchitecture().SendCommand<T>();
        }
        
        public static void SendCommand<T>(this ICanSendCommand self, T command) where T : IAddCountCommand
        {
            self.GetArchitecture().SendCommand<T>(command);
        }
    }
}

