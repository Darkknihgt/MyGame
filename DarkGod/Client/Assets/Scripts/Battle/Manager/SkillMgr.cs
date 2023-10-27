/****************************************************
    文件：SkillMgr.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/24 22:6:30
	功能：技能管理器
*****************************************************/
using System.Collections.Generic;
using UnityEngine;

public class SkillMgr : MonoBehaviour
{
    TimerSvc timerSvc = null;

    private ResSvc reSvc;
    public void Init()
    {
        timerSvc = TimerSvc.Instance;
        reSvc = ResSvc.Instance;
        PECommon.Log("Init SkillMgr");
    }


    /// <summary>
    /// 技能效果表现
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="skillID"></param>
    public void AttackEffect(EntityBase entity, int skillID)
    {
        SkillConfigs skillData = reSvc.GetSkillInfo(skillID);

        if (!skillData.isCollide)
        {
            //忽略角色之间的刚体碰撞
            Physics.IgnoreLayerCollision(9, 10);
            timerSvc.AddTimeTask((int tid) =>
            {
                Physics.IgnoreLayerCollision(9, 10, false);
            }, skillData.skillTime);

        }

        if (entity.entityType == EntityType.Player)
        {
            if (entity.GetInputDir() == Vector2.zero)
            {
                //TODO 自动搜索最近的怪物
                Vector2 dir = entity.CalcTargetDir();
                if (dir != Vector2.zero)
                {
                    entity.SetAtkRotation(dir);
                }
            }
            else
            {
                entity.SetAtkRotation(entity.GetInputDir(), true);
            }
        }

        entity.SetAction(skillData.aniAction);
        entity.SetFx(skillData.fx, skillData.skillTime);

        #region 节能位移控制
        int sum = 0; //用来计算时间轴
        for (int i = 0; i < skillData.skillMoveLst.Count; i++)
        {
            SkillMoveConfigs skillMoveData = reSvc.GetSkillMoveInfo(skillData.skillMoveLst[i]);

            float skillMoveSpeed = skillMoveData.moveDis / (skillMoveData.moveTime / 1000f);
            //技能延迟
            if (skillMoveData.delayTime > 0)
            {
                sum += skillMoveData.delayTime;
                int moveid = timerSvc.AddTimeTask((int tid) =>
                {
                    entity.SkillMoveState(true, skillMoveSpeed);
                    entity.RemoveCB(tid);
                }, sum);
                entity.SkillMoveCBLst.Add(moveid);
            }
            else
            {
                entity.SkillMoveState(true, skillMoveSpeed);
            }
            sum += skillMoveData.moveTime;
            int stopid = timerSvc.AddTimeTask((int tid) =>
            {
                entity.SkillMoveState(false);
                entity.RemoveCB(tid);
            }, sum);
            entity.SkillMoveCBLst.Add(stopid);
        }
        //sum = 0;

        #endregion
        entity.canControl = false;
        entity.SetDir(Vector2.zero);

        //进入霸体状态
        if (!skillData.isBreak)
        {
            entity.entityState = EntityState.BatiState;
        }

        entity.skEndCB = timerSvc.AddTimeTask((int tid) =>
        {
            entity.Idle();
        }, skillData.skillTime);
    }

    /// <summary>
    /// 技能伤害
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="skillID"></param>
    public void AttackDamage(EntityBase entity, int skillID)
    {
        //获取到技能数据
        SkillConfigs skillData = reSvc.GetSkillInfo(skillID);

        int sum = 0;
        //对其中的skillActionLst进行遍历
        for (int i = 0; i < skillData.skillActionLst.Count; i++)
        {
            int index = i;
            SkillActionConfigs skillActionCfg = reSvc.GetSkillAtionInfo(skillData.skillActionLst[i]);
            sum += skillActionCfg.delayTime;
            int actid = timerSvc.AddTimeTask((int tid) =>
            {
                if(entity != null)
                {
                    SkillAction(entity, skillData, index);
                    entity.RemoveActionCB(tid);
                }                
            }, sum);
            entity.SkillActionCBLst.Add(actid);

        }
    }

    /// <summary>
    /// 处理具体伤害计算逻辑
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="actionID"></param>
    public void SkillAction(EntityBase entity, SkillConfigs skillData, int index)
    {
        SkillActionConfigs skillActionCfg = reSvc.GetSkillAtionInfo(skillData.skillActionLst[index]);
        int damage = skillData.skillDamageLst[index];

        if (entity.entityType == EntityType.Monster)
        {
            EntityPlayer target = entity.battleMgr.entitySelfPlayer;

            //当玩家被其他怪物打死时
            if (target == null) return;

            if (InRange(entity.GetPos(), target.GetPos(), skillActionCfg.radius) &&
                    InAngle(entity.GetTrans(), target.GetPos(), skillActionCfg.angle))
            {
                //伤害计算TODO
                CalcDamage(entity, target, skillData, (int)skillData.dmgType);
            }
        }
        else if (entity.entityType == EntityType.Player)
        {

            //获取所有怪物的实体信息，用来遍历计算
            List<EntityMonster> ml = entity.battleMgr.GetEntityMonster();
            for (int i = 0; i < ml.Count; i++)
            {
                EntityMonster target = ml[i];
                //对怪物与玩家的距离进行判定
                if (InRange(entity.GetPos(), target.GetPos(), skillActionCfg.radius) &&
                    InAngle(entity.GetTrans(), target.GetPos(), skillActionCfg.angle))
                {
                    //伤害计算TODO
                    CalcDamage(entity, target, skillData, (int)skillData.dmgType);
                }
            }
        }

    }

    public bool InRange(Vector3 from, Vector3 to, float range)
    {
        float dis = Vector3.Distance(from, to);
        if (dis <= range) //说明在范围内
        {
            return true;
        }
        return false;
    }

    public bool InAngle(Transform trans, Vector3 to, float angle)
    {
        Vector3 start = trans.forward;
        Vector3 target = to.normalized;
        float ang = Vector3.Angle(start, to);
        if (ang <= angle / 2)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 用来计算技能伤害的数学逻辑,内部被调用
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="damage"></param>
    private void CalcDamage(EntityBase caster, EntityBase target, SkillConfigs skillCfg, int damage)
    {
        int dmgSum = damage; //技能设定的初始伤害
        if (skillCfg.dmgType == DamageType.AD)
        {
            //闪避
            int dodgeNum = PETools.RDInt(1, 100);
            if (target.Props.dodge >= dodgeNum)
            {
                PECommon.Log("闪避Rate" + dodgeNum + "/" + target.Props.dodge);
                target.SetDodge();
                return;
            }

            //属性加成
            dmgSum += caster.Props.ad;

            //暴击
            if (caster.Props.critical > PETools.RDInt(1, 100))
            {
                float criticalRate = 1 + (PETools.RDInt(1, 100) / 100.0f);
                dmgSum = (int)(dmgSum * criticalRate);
                target.SetCritical(dmgSum);
                PECommon.Log("暴击");
            }

            //计算穿甲
            int addef = (int)((1 - caster.Props.pierce / 100.0f) * target.Props.addef);
            dmgSum -= addef;
        }
        else if (skillCfg.dmgType == DamageType.AP)
        {
            //计算属性加成
            dmgSum += caster.Props.ap;
            //计算魔法抗性
            dmgSum -= caster.Props.apdef;
        }

        //最后判定，伤害小于0时
        if (dmgSum <= 0)
        {
            dmgSum = 1;
        }
        target.SetHurt(dmgSum);

        if (target.HP - dmgSum < 0)
        {
            target.HP = 0;
            target.Die();

            if(target.entityType == EntityType.Monster)
            {
                target.battleMgr.RemoveMonster(target.Name);
            }else if(target.entityType == EntityType.Player)
            {
                target.battleMgr.EndBattle(false,0);
                target.battleMgr.entitySelfPlayer = null;
            }
        }
        else
        {
            target.HP -= dmgSum;
            //播放受到攻击动画
            if (target.entityState == EntityState.None && target.GetBreakState())
            {
                target.Hit();
            }
        }

    }

    //优化中转站，技能伤害和技能效果都是由此来调用
    public void SkillAttack(EntityBase entity, int skillID)
    {
        entity.SkillMoveCBLst.Clear();
        entity.SkillActionCBLst.Clear();

        AttackDamage(entity, skillID);
        AttackEffect(entity, skillID);
    }
}

