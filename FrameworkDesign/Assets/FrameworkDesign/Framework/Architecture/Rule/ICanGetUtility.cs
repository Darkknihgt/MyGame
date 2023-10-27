using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace FrameworkDesign 

{
    public interface ICanGetUtility :IBelongToArchitect
    {
        
    }

    public static class CanGetUtilityExtension
    {
        public static T GetUtility<T>(this ICanGetUtility self) where T : class, IUtility
        {
            return self.GetArchitecture().GetUtility<T>();
        }
    }

    

}

