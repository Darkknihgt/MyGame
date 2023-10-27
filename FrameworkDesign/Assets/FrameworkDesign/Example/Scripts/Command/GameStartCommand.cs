using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CounterApp;
using FrameworkDesign;


namespace FrameworkDesign.Example
{
    public class GameStartCommand :AbstactCommand,IAddCountCommand
    {
        protected override void OnExecute()
        {
            this.SendEvent<EventGamePanel>();
            
        }
          
       
    }
}

