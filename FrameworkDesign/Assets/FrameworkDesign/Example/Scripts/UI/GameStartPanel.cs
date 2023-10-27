using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameworkDesign;
using CounterApp;

namespace FrameworkDesign.Example
{
    public class GameStartPanel : MonoBehaviour,IController
    {
        IArchitecture IBelongToArchitect.GetArchitecture()
        {
            return PointGame.Interface;
        }

        //public GameObject Enemies;
        void Start()
        {
            transform.Find("Button").GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    gameObject.SetActive(false);

                    //new GameStartCommand().Execute();
                    this.SendCommand<GameStartCommand>();
            });
        }
    }
}

