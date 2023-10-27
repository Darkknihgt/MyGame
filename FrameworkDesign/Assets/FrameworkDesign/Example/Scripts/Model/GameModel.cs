using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameworkDesign;

namespace FrameworkDesign.Example
{
    public interface IGameModel :IModel
    {
        public BindableProperty<int> KillCount { get; }

        public BindableProperty<int> Gold { get; }

        public BindableProperty<int> Score { get; }

        public BindableProperty<int> BestScore { get; }




    }



    public class GameModel :AbstractModel, IGameModel
    {
        //public static int killCount = 0;

        //private GameModel() { }




        public BindableProperty<int> KillCount { get; } = new BindableProperty<int>()
        {
            Value = 0
        };
        public BindableProperty<int> Gold { get; } = new BindableProperty<int>()
        {
            Value = 0
        };
        public BindableProperty<int> Score { get; } = new BindableProperty<int>()
        {
            Value = 0
        };
        public BindableProperty<int> BestScore { get; } = new BindableProperty<int>()
        {
            Value = 0
        };

        protected override void OnInit()
        {
            
        }
    }
}

