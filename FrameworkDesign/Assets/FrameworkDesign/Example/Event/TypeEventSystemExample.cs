using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameworkDesign.Example
{
    public class TypeEventSystemExample : MonoBehaviour
    {
       
        public struct EventA
        {

        }

        public struct EventB
        {
            public int ParamB;
        }

        public interface IEventGroup
        {

        }

        public struct EventC : IEventGroup
        {

        }
        public struct EventD : IEventGroup
        {

        }

        private TypeEventSystem mTypeEventSystem = new TypeEventSystem();

        private void Start()
        {
            mTypeEventSystem.Register<EventA>(OnEventA);
            mTypeEventSystem.Register<EventB>(b =>
               {
                   Debug.Log("OnEventB :" + b.ParamB);
               }
                ).UnRegisterWhenGameObjectDestroyed(gameObject);


                mTypeEventSystem.Register<IEventGroup>(group =>
               {
                   Debug.Log(group.GetType());
               }
                ).UnRegisterWhenGameObjectDestroyed(gameObject);

            
        }

        private void OnEventA(EventA obj)
        {
            Debug.Log("OnEventA");
        }

        private void Update()
        {
            if (Input.GetMouseButton(0)){
                mTypeEventSystem.Send<EventA>();
            }
            if (Input.GetMouseButton(1))
            {
                mTypeEventSystem.Send(new EventB()

                {
                    ParamB = 123
                });
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                mTypeEventSystem.Send<IEventGroup>(new EventC());
                mTypeEventSystem.Send<IEventGroup>(new EventD());
            }
        }
    }

}

