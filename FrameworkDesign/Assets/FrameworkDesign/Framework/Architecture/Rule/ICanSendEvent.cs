using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FrameworkDesign
{

    public interface ICanSendEvent : IBelongToArchitect
    {
       
    }

    public static class CanSendEventExtension
    {
        public static void SendEvent<T> (this ICanSendEvent self) where T : new()
        {
            self.GetArchitecture().SendEvent<T>();
        }
        
        public static void SendEvent<T> (this ICanSendEvent self,T e)
        {
            self.GetArchitecture().SendEvent<T>(e);
        }
    }
}

