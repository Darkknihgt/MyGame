using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameworkDesign.Example
{
    public class GameListener : MonoBehaviour,IController       //������յ����¼�
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

