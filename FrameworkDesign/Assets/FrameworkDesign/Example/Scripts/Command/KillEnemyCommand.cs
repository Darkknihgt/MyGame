using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CounterApp;

namespace FrameworkDesign.Example
{
    public class KillEnemyCommand : AbstactCommand
    {
      
        protected override void OnExecute()
        {
           this.GetModel<IGameModel>().KillCount.Value++;
            if (PointGame.Get<IGameModel>().KillCount.Value > 9)
            {
                this.SendEvent<EventGamePass>();
                
            }
        }
    }
}

