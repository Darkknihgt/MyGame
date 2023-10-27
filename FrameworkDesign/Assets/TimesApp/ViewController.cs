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
        private ICounterModel mCounterModel;   //�������ݲ������


        

        void Start()
        {
            mCounterModel = this.GetModel<ICounterModel>();  //���һ�����ݲ�����͵�ʵ��
            

            //��model��������ί�У����������ί��
            mCounterModel.Count.OnValueChanged += OnCountChanged; // ���ֲ�
            OnCountChanged(mCounterModel.Count.Value); //��ʼ��ʱ������ʾһ��
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

