
using System;

namespace FrameworkDesign
{
    public interface ICanRegisterEvent : IBelongToArchitect
    {
        
    }

    public static class CanRegisterEventExtnsion
    {
        public static IUnRegister RegisterEvent<T>(this ICanRegisterEvent self,Action<T> onEvent)
        {
            return self.GetArchitecture().RegisterEvent<T>(onEvent);
        }

        public static void UnRegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent)
        {
            self.GetArchitecture().UnRegisterEvent<T>(onEvent);
        }
    }
}

