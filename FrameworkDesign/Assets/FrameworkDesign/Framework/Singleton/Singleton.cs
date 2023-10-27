using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace FrameworkDesign
{
    public class Singleton<T> where T : class
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
               if(_instance == null)
                {
                    var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);//��ȡ�޲η�public�Ĺ��캯��
                    var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

                    if(ctor == null)
                    {
                        throw new Exception("Non-public Constructor() not found in" + typeof(T));
                    }
                    _instance = ctor.Invoke(null) as T;
                }
                return _instance;
            }
        }
            

    }
}

