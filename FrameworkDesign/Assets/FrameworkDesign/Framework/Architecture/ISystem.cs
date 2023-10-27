using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FrameworkDesign 
{
    public interface ISystem : IBelongToArchitect,ICanSetArchitecture,ICanGetModel,ICanGetUtility,ICanSendEvent,ICanRegisterEvent
    {
        void Init(); 
    }

    public abstract class AbstractSystem : ISystem
    {
        private IArchitecture mArchitecture;
        IArchitecture IBelongToArchitect.GetArchitecture()
        {
            return mArchitecture;
        }


        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            mArchitecture = architecture;
        }
        void ISystem.Init()
        {
            OnInit();
        }

        protected abstract void OnInit();
    }
}

