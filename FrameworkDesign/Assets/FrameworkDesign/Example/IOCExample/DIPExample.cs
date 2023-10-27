using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FrameworkDesign
{
    public class DIPExample : MonoBehaviour
    {
      public interface IStorage
        {
            void SaveString(string key, string value);
            string LoadString(string key, string defaulValue = "");
        }
        public class PlayerPrefsStorage : IStorage
        {
            public void SaveString(string key, string value)
            {
                PlayerPrefs.SetString(key, value);
            }

            public string LoadString(string key, string defaulValue = "")
            {
                return PlayerPrefs.GetString(key, defaulValue);
            }

        }

        private void Start()
        {
            var constainer = new IOCContainer();
            constainer.Register<IStorage>(new PlayerPrefsStorage());
            var storage = constainer.Get<IStorage>();

            storage.SaveString("name", "‘À–– ±¥Ê¥¢");
            Debug.Log(storage.LoadString("name"));


        }
    }
}

