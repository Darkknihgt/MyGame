using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace FrameworkDesign
{
    public class IOCContainer 
    {
        Dictionary<Type, object> mInstance = new Dictionary<Type, object>(); //类的类型作为key，实例作为value

        //注册的API
        public void Register<T>(T instance)
        {
            var key = typeof(T);
            if (mInstance.ContainsKey(key))    //查询字典中是否包含该key
            {
                mInstance[key] = instance;    //如果有该key，则把实例赋给它
            }
            else
            {
                mInstance.Add(key, instance);
            }
        }

        //获取API
        public T Get<T>() where T : class
        {
            var key = typeof(T);
            object retInstance;
            if (mInstance.TryGetValue(key,out retInstance))
            {
             return retInstance as T;  //存储的时候隐式转换成了object类型，取出来的时候则转会对应的T的实例
            }
            return null;
        }
    }
}

