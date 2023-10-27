using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CounterApp;

namespace FrameworkDesign
{
    public interface IAchievementSystem : ISystem
    {
        
    }

    public class AchievementSystem :AbstractSystem, IAchievementSystem
    {
       

        protected override void OnInit()
        {
            var countModel = this.GetModel<ICounterModel>();

            var previousCount = countModel.Count.Value;
            countModel.Count.OnValueChanged += newCount =>
            {
                if (previousCount < 10 && newCount >= 10)
                {
                    Debug.Log("解锁点击10次成就");
                }
                else if (previousCount < 20 && newCount >= 20)
                {
                    Debug.Log("解锁点击20次成就");
                }
                previousCount = newCount;
            };
        }
    }
}

