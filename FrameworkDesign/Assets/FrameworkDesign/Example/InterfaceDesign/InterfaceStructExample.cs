using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FrameworkDesign.Example
{



    public class InterfaceStructExample : MonoBehaviour
    {
        interface ICustomScript
        {
            void Start();
            void Update();
            void Destroy();
        }
        public abstract class CustomScript : ICustomScript
        {

            protected bool mStarted { get; private set; }
            protected bool mDestroyed { get; private set; }
            void ICustomScript.Destroy()
            {
                OnDestroy();
                mDestroyed = true;
            }

            void ICustomScript.Start()
            {
                Onstart();
                mStarted = true;
            }

            void ICustomScript.Update()
            {
                OnUpdate();
            }

            protected abstract void Onstart();
            protected abstract void OnUpdate();
            protected abstract void OnDestroy();
            public void Do1() { }
        }

        class MyScript : CustomScript
        {
            protected override void OnDestroy()
            {
                Debug.Log("ondestroy");
            }

            protected override void Onstart()
            {
                Debug.Log("onstart");
            }

            protected override void OnUpdate()
            {
                Debug.Log("onupdate");
            }
        }

        private void Start()
        {
            ICustomScript myScript = new MyScript();
            myScript.Start();
            myScript.Update();
            myScript.Destroy();
            
            
        }
    }
}

