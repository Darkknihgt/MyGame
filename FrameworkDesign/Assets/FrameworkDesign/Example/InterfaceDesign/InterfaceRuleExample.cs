using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace FrameworkDesign.Example
{
    /// <summary>
    /// 接口+扩展时用于限制方法的访问规则
    /// </summary>
    /// 
    public class CanDoEveryThing
    {
        public void DoSomething1()
        {
            Debug.Log("DoSomething1");
        } 
            
        public void DoSomething2()
        {
            Debug.Log("DoSomething2");
        } 
        
        public void DoSomething3()
        {
            Debug.Log("DoSomething3");
        }
    }

    public interface IHasEverything
    {
        CanDoEveryThing canDoEveryThing { get; }
        
    }

    public interface ICanDoSomething1 : IHasEverything
    {
        
    }
    public static class ICanDoSomething1Extension
    {
        public static void DoSomething123(this ICanDoSomething1 self)
        {
            self.canDoEveryThing.DoSomething1();
        }
    }

    public interface ICanDoSomething2 : IHasEverything
    {

    }
    public static class ICanDoSomething2Extension
    {
        public static void DoSomething2(this ICanDoSomething2 self)
        {
            self.canDoEveryThing.DoSomething2();
        }
    }

    public interface ICanDoSomething3 : IHasEverything
    {

    }
    public static class ICanDoSomething3Extension
    {
        public static void DoSomething3(this ICanDoSomething3 self)
        {
            self.canDoEveryThing.DoSomething3();
        }
    }



    public class InterfaceRuleExample : MonoBehaviour
    {
        public class OnlyCanDo1 : ICanDoSomething1
        {
           CanDoEveryThing  IHasEverything.canDoEveryThing { get; } = new CanDoEveryThing();

        }

        public class OnlyCanDo3 : ICanDoSomething3
        {
            CanDoEveryThing IHasEverything.canDoEveryThing { get; } = new CanDoEveryThing();
            public void Do1() { }
            public void Do2() { }
        }

        private void Start()
        {
            var onlyCanDo1 = new OnlyCanDo1();
            onlyCanDo1.DoSomething123();
            var onlyCanDo3 = new OnlyCanDo3();
            onlyCanDo3.DoSomething3();
        }
    }
}

