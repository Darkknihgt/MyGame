using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace FrameworkDesign
{
    public class IOCContainer 
    {
        Dictionary<Type, object> mInstance = new Dictionary<Type, object>(); //���������Ϊkey��ʵ����Ϊvalue

        //ע���API
        public void Register<T>(T instance)
        {
            var key = typeof(T);
            if (mInstance.ContainsKey(key))    //��ѯ�ֵ����Ƿ������key
            {
                mInstance[key] = instance;    //����и�key�����ʵ��������
            }
            else
            {
                mInstance.Add(key, instance);
            }
        }

        //��ȡAPI
        public T Get<T>() where T : class
        {
            var key = typeof(T);
            object retInstance;
            if (mInstance.TryGetValue(key,out retInstance))
            {
             return retInstance as T;  //�洢��ʱ����ʽת������object���ͣ�ȡ������ʱ����ת���Ӧ��T��ʵ��
            }
            return null;
        }
    }
}

