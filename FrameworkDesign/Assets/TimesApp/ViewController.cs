using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameworkDesign;

namespace CounterApp
{
    public class ViewController : MonoBehaviour,IController
    {
        private ICounterModel mCounterModel;   //声明数据层的类型


        

        void Start()
        {
            mCounterModel = this.GetModel<ICounterModel>();  //获得一个数据层的类型的实例
            

            //在model类中声明委托，在这里添加委托
            mCounterModel.Count.OnValueChanged += OnCountChanged; // 表现层
            OnCountChanged(mCounterModel.Count.Value); //初始的时候先显示一次
            transform.Find("BtnAdd").GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    this.SendCommand<AddCountCommand>();
                    //new AddCountCommand().Execute();
                    
                });

            transform.Find("BtnSub").GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    this.SendCommand<SubCountCommand>();
                    //new SubCountCommand().Execute();
                    
                });
            
        }


        private void OnCountChanged(int obj)
        {
            transform.Find("Score").GetComponent<Text>().text = obj.ToString();
            //print(i);
            //i++;
        }

        private void OnDestroy()
        {
            mCounterModel.Count.OnValueChanged -= OnCountChanged;
            mCounterModel = null;
        }

        IArchitecture IBelongToArchitect.GetArchitecture()
        {
            return CountApp.Interface;
        }
    }
}

