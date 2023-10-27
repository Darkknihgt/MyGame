using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameworkDesign;

namespace CounterApp
{
    public interface IAddCountCommand :IBelongToArchitect,ICanSetArchitecture,ICanGetSystem,ICanGetModel,ICanGetUtility,ICanSendEvent,ICanSendCommand
    {
        void Execute();
    }


    public abstract class AbstactCommand : IAddCountCommand
    {

        private IArchitecture mArchitecture;
         void IAddCountCommand.Execute()
        {
            OnExecute();
        }

        IArchitecture IBelongToArchitect.GetArchitecture()
        {
            return mArchitecture;
        }

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            mArchitecture = architecture;
        }


        protected abstract void OnExecute();
    }
}

