/****************************************************
    文件：StateIdle.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/25 12:58:7
	功能：站立状态
*****************************************************/

using UnityEngine;
public class StateIdle : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentAniState = AniState.Idle;
        entity.SetDir(Vector2.zero);
        entity.skEndCB = -1;
        //PECommon.Log("enter idle");
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        //PECommon.Log("exit idle");
    }

    public void Process(EntityBase entity, params object[] args)
    {
        //处于idle状态时符合连击要求
        if (entity.nextSkillID != 0)
        {
            entity.Attack(entity.nextSkillID);
        }
        else
        {
            if (entity.entityType == EntityType.Player)
            {
                entity.canRlsSkill = true;
            }
            if (entity.GetInputDir() != Vector2.zero)
            {
                entity.Move();
                entity.SetDir(entity.GetInputDir());
            }
            else
            {
                entity.SetBlend(Constants.idleAnim);
            }
            //PECommon.Log("process idle");
        }
    }
}

