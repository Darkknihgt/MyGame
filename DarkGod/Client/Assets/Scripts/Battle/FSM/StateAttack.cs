/****************************************************
    文件：StateAttack.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/27 0:9:30
	功能：战斗状态
*****************************************************/

using UnityEngine;

public class StateAttack : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentAniState = AniState.Attack;
        entity.curtSkillCfg = ResSvc.Instance.GetSkillInfo((int)args[0]);
        //PECommon.Log("enter attack");
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        entity.ExitCurtSkill();
        //PECommon.Log("exit attack");

    }

    public void Process(EntityBase entity, params object[] args)
    {
        if(entity.entityType == EntityType.Player)
        {
            entity.canRlsSkill = false;
        }
        //技能伤害计算,技能效果
        entity.SkillAttack(entity, (int)args[0]);
        //PECommon.Log("process attack");
    }
}