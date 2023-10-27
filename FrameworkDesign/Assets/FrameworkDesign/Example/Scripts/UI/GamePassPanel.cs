using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FrameworkDesign.Example
{
    public class GamePassPanel :MonoBehaviour,IController
    {
        
        private void Start()
        {
            this.RegisterEvent<EventGamePass>(OnGamePass);
            //EventKilledEnemy.Register(OnEnemyKilled);
            //GameModel.KillCount.OnValueChanged += OnEnemyKilled;
        }

        //private void OnEnemyKilled(int value)
        //{
        //    //GameModel.KillCount.Value++;
        //    //print(killedEnemyCount);
        //    if (GameModel.KillCount.Value > 9)
        //    {
        //        //GamePasspanel.SetActive(true);
        //        EventGamePass.Trigger();
        //    }

        //}

        private void OnGamePass(EventGamePass e)
        {
            transform.Find("Canvas/GamePassPanel").gameObject.SetActive(true);
            //throw new NotImplementedException();
        }
        private void OnDestroy()
        {
            this.UnRegisterEvent<EventGamePass>(OnGamePass);
            //GameModel.KillCount.OnValueChanged -= OnEnemyKilled;
        }

        public IArchitecture GetArchitecture()
        {
            return PointGame.Interface;
        }
    }
}

