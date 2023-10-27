using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameworkDesign;

namespace CounterApp
{
    public class CountApp : Architecture<CountApp>
    {
        protected override void Init()
        {
            RegisterSystem<IAchievementSystem>(new AchievementSystem());
            RegisterModel<ICounterModel>(new GameModel());
            Register<IStorage>(new PlayerPrefsStorage());
        }
    }
}

