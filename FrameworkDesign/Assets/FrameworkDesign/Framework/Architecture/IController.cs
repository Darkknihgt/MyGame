using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FrameworkDesign
{
    public interface IController : IBelongToArchitect,ICanSendCommand,ICanGetSystem,ICanGetModel,ICanRegisterEvent
    { 

    }

}
