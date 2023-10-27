/****************************************************
    文件：StateDie.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/8/2 11:22:48
	功能：死亡状态
*****************************************************/

using UnityEngine;

public class StateDie : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentAniState = AniState.Die;
        entity.RmvSkillCB();
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        
    }

    public void Process(EntityBase entity, params object[] args)
    {
        entity.SetAction(Constants.ActionDie);
        if (entity.entityType == EntityType.Monster)
        {
            entity.GetCC().enabled = false;
            TimerSvc.Instance.AddTimeTask((int tid) =>
            {
                entity.GetTrans().gameObject.SetActive(false);

            }, Constants.DieAniLength);
        }
    }
}