/****************************************************
    文件：StateIdle.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/25 12:58:7
	功能：移动状态
*****************************************************/

public class StateMove : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentAniState = AniState.Move;
        //PECommon.Log("enter move");
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        //PECommon.Log("exit move");

    }

    public void Process(EntityBase entity, params object[] args)
    {
        //PECommon.Log("process move");
        entity.SetBlend(Constants.walkAnim);
    }
}

