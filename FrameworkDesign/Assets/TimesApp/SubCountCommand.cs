using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameworkDesign;


namespace CounterApp
{
    public class SubCountCommand : AbstactCommand
    {
        

        protected override void OnExecute()
        {
            this.GetModel<ICounterModel>().Count.Value--;
        }
    }

}
