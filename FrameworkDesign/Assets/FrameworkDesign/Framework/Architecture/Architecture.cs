using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CounterApp;
using FrameworkDesign.Example;
using System;

namespace FrameworkDesign
{
    public interface IArchitecture
    {
        void RegisterSystem<T>(T system) where T : ISystem;
        void RegisterModel<T>(T model) where T : IModel;

        void RegisterUtility<T>(T utility) where T : IUtility;

        T GetModel<T>() where T : class, IModel;
        T GetSystem<T>() where T : class, ISystem;

        T GetUtility<T>() where T : class,IUtility;

        void SendCommand<T>() where T : IAddCountCommand, new();
        void SendCommand<T>(T command) where T : IAddCountCommand;

        void SendEvent<T>() where T : new();
        void SendEvent<T>(T e);

        IUnRegister RegisterEvent<T>(Action<T> onEvent);
        void UnRegisterEvent<T>(Action<T> onEvent);

    }


    public abstract class Architecture<T> :IArchitecture where T : Architecture<T>,new()
    {
        /// <summary>
        /// �Ƿ��ʼ�����
        /// </summary>
        private bool mInited = false;

        private List<IModel> mModels = new List<IModel>();
        private List<ISystem> mSystems = new List<ISystem>();

        public void RegisterModel<T>(T model) where T : IModel
        {

            model.SetArchitecture(this);  //����model�ļܹ���Ϊ��ǰ
            mContainer.Register<T>(model);
            if (!mInited)
            {
                mModels.Add(model);
            }
            else
            {
                model.Init();
            }
        }
        #region ���Ƶ���ģʽ ���ǽ����ڲ��ɷ���

        private static T mArchitecture = null;

        public static IArchitecture Interface
        {
            get
            {
                if(mArchitecture == null)
                {
                    MakeSureArchitect();
                }
                return mArchitecture;
            }
        }




        static void MakeSureArchitect()
        {
            if(mArchitecture == null)
            {
                mArchitecture = new T();
                mArchitecture.Init();

                //����
                
                
                foreach (var architectureModel in mArchitecture.mModels)
                {
                    architectureModel.Init();   //��GameModel��������ݽ��г�ʼ��
                }
                mArchitecture.mModels.Clear();
                foreach (var architectureSystem in mArchitecture.mSystems)
                {
                    architectureSystem.Init();   //��System��������ݽ��г�ʼ��
                }
                
                mArchitecture.mSystems.Clear();
                mArchitecture.mInited = true;
            }
        }
        #endregion
        private IOCContainer mContainer = new IOCContainer();

        //��������ע��ģ��
        protected abstract void Init();

        //�ṩһ����ȡģ���API
        public static T Get<T>() where T : class
        {
            MakeSureArchitect();
            return mArchitecture.mContainer.Get<T>();
        }

        //�����������������ע��
        public static void Register<T>(T instance)
        {
            MakeSureArchitect();
            mArchitecture.mContainer.Register<T>(instance);
        }


        public T GetModel<T>() where T :class,IModel
        {
            return mContainer.Get<T>();
        }


        public T GetUtility<T>() where T : class,IUtility
        {
            return mContainer.Get<T>();
        }

        public void RegisterUtility<T>(T utility) where T : IUtility
        {
            mContainer.Register<T>(utility); 
        }

        public void RegisterSystem<T>(T system) where T : ISystem
        {
            system.SetArchitecture(this) ;
            mContainer.Register<T>(system);

            if (!mInited)
            {
                mSystems.Add(system);
            }
            else
            {
                system.Init();
            }
        }

        public void SendCommand<T>() where T : IAddCountCommand, new()
        {
            var command = new T();
            command.SetArchitecture(this);
            command.Execute();
           // command.SetArchitecture(null);
        }

        public void SendCommand<T>(T command) where T: IAddCountCommand
        {
            command.SetArchitecture(this);
            command.Execute();
        }

        T IArchitecture.GetSystem<T>()
        {
            return mContainer.Get<T>();
        }


        private ITypeEventSystem mTypeEventSystem = new TypeEventSystem();
        public void SendEvent<T>() where T : new()
        {
            mTypeEventSystem.Send<T>();
        }


       
        public void SendEvent<T>(T e)
        {
            mTypeEventSystem.Send<T>(e);
        }

        public IUnRegister RegisterEvent<T>(Action<T> onEvent)
        {
            return mTypeEventSystem.Register<T>(onEvent);
        }

        public void UnRegisterEvent<T>(Action<T> onEvent)
        {
            mTypeEventSystem.UnRegister<T>(onEvent);
        }
    }

}

