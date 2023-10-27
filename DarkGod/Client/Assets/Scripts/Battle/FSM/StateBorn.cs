/****************************************************
    文件：StateBorn.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/8/2 0:8:29
	功能：出生状态
*****************************************************/

using UnityEngine;

public class StateBorn : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentAniState = AniState.Born;
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        
    }

    public void Process(EntityBase entity, params object[] args)
    {
        //播放出生动画
        entity.SetAction(Constants.ActionBorn);
        TimerSvc.Instance.AddTimeTask((int id)=> {
            entity.SetAction(Constants.ActionDefault);
        },700);
    }
}