using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FrameworkDesign.Example
{
    interface ICanSayHello 
    {
        void SayHello();
        void SayOther();
    }
    public class InterfaceDesignExample : MonoBehaviour, ICanSayHello
    {
        public void SayHello()
        {
            Debug.Log("hello");
        }

         void ICanSayHello.SayOther()
        {
            Debug.Log("other");
        }

        private void Start()
        {
            this.SayHello();
            (this as ICanSayHello).SayOther();
        }
    }

    
}

