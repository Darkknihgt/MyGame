using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FrameworkDesign.Example
{
    public class IOCExample : MonoBehaviour
    {
        private void Start()
        {
            var container = new IOCContainer();
            container.Register<IBluetoothManager>(new BluetoothManager()); //��BluetoothManagerע�ᵽIOC�ֵ���

            var bluetoothManager = container.Get<IBluetoothManager>();  //��IOC�ֵ�����ȡһ��BlueToothManager��ʵ��

            bluetoothManager.Connect();
        }

        public interface IBluetoothManager
        {
            void Connect();
        }


        public class BluetoothManager:IBluetoothManager
        {
            public void Connect()
            {
                Debug.Log("�������ӳɹ���");
            }
        }
    }
}

