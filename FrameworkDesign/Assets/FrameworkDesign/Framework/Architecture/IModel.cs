using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameworkDesign.Example;
using CounterApp;

namespace FrameworkDesign
{
    public interface IModel :IBelongToArchitect,ICanSetArchitecture,ICanGetUtility,ICanSendEvent
    {
        void Init();
    }

    public abstract class AbstractModel : IModel
    {
        private IArchitecture mArchitecture;
        IArchitecture  IBelongToArchitect.GetArchitecture()
        {
            return mArchitecture;
        }
        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            mArchitecture = architecture;
        }

        void IModel.Init()
        {
            OnInit();
        }

        protected abstract void OnInit();
    }

}
