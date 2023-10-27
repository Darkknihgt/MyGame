using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameworkDesign;

 namespace FrameworkDesign.Example
{
    public class PointGame : Architecture<PointGame>
    {
        //private static IOCContainer mContainer;  // ÏÈÊÇ¾²Ì¬ÉùÃ÷ÈÝÆ÷
        
        //static void MakeSureContainer()
        //{
        //    if(mContainer == null)
        //    {
        //        mContainer = new IOCContainer();
        //        Init();
        //    }
        //}

        protected override void Init()
        {
            Register<IGameModel>( new GameModel());
        }

        //public static T Get<T>() where T : class 
        //{
        //    MakeSureContainer();
        //    return mContainer.Get<T>();
        //}
    }
}

