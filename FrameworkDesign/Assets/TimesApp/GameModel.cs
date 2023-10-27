using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameworkDesign;


namespace CounterApp
{

    public interface ICounterModel:IModel
    {
        BindableProperty<int> Count { get; }
    }
    public class GameModel :AbstractModel, ICounterModel
    {
       // private GameModel() { }
  
       
        
        

        protected override void OnInit()
        {
            //通过Architecture获取
            var storage = this.GetUtility<IStorage>();
            Count.Value = storage.LoadInt("COUNTER_COUNT", 0);
            Count.OnValueChanged += count => { storage.SaveInt("COUNTER_COUNT", count); };
            
            
        }

        public BindableProperty<int> Count { get; } = new BindableProperty<int>()
        {
            Value = 0
        };
        

      
    }
}

