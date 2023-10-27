using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameworkDesign.Example
{
    public class GameListener : MonoBehaviour,IController       //处理就收到的事件
    {
        private void Awake()
        {
            this.RegisterEvent<EventGamePanel>(OnGameStart);
        }

        private void OnGameStart(EventGamePanel e)
        {
            transform.Find("Enemies").gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
           this.UnRegisterEvent<EventGamePanel>(OnGameStart);
        }

        public IArchitecture GetArchitecture()
        {
            return PointGame.Interface;
        }
    }
}

