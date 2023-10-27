using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FrameworkDesign.Example
{
    public class DestroyEnemy : MonoBehaviour,IController
    {
        IArchitecture IBelongToArchitect.GetArchitecture()
        {
            return PointGame.Interface;
        }

        //public  GameObject GamePasspanel;
        private void OnMouseDown()
        {
            Destroy(gameObject);
            //EventKilledEnemy.Trigger();
            // GameModel.KillCount.Value++;
            //new KillEnemyCommand().Execute(); //实例化后调用类中方法
            this.SendCommand<KillEnemyCommand>();
        }

    }

}
