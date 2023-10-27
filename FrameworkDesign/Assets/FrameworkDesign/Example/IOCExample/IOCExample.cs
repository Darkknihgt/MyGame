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
            container.Register<IBluetoothManager>(new BluetoothManager()); //把BluetoothManager注册到IOC字典中

            var bluetoothManager = container.Get<IBluetoothManager>();  //从IOC字典中提取一个BlueToothManager的实例

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
                Debug.Log("蓝牙链接成功！");
            }
        }
    }
}

