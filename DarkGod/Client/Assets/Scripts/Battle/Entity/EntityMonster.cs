/****************************************************
    文件：EntityMonster.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/30 16:21:36
	功能：怪物实体数据类
*****************************************************/

using UnityEngine;

public class EntityMonster : EntityBase
{

    public MonsterData md;

    public EntityMonster()
    {
        entityType = EntityType.Monster;
    }


    public override void SetBattleProps(BattleProps props)
    {
        int level = md.mLevel;
        BattleProps p = new BattleProps()
        {
            hp = props.hp * level,
            ad = props.ad * level,
            ap = props.ap * level,
            addef = props.addef * level,
            apdef = props.apdef * level,
            dodge = props.dodge * level,
            pierce = props.pierce * level,
            critical = props.critical * level,

        };
        this.Props = p;
        this.HP = p.hp;
    }


    #region 怪物AI逻辑

    private bool runAI = true;
    private float checkCountTime = 0;
    private float checkTime = 2.0f;
    private float atkCountTime = 0;
    private float atkTime = 2.0f;
    public override void TickAILogic()
    {
        if (!runAI)
        {
            return;
        }
        if (currentAniState == AniState.Idle || currentAniState == AniState.Move) //只有当AI诞生后处于idle或move状态时，才进行计算
        {
            if (battleMgr.isPauseGame)
            {
                Idle();
                return;
            }
            float delta = Time.deltaTime;
            checkCountTime += delta;
            if (checkCountTime < checkTime)
            {
                return;
            }
            else
            {
                //计算目标位置
                Vector2 dir = CalcTargetDir();
                if (!InAtkRange())
                {
                    SetDir(dir);
                    Move();
                }
                else
                {
                    SetDir(Vector2.zero);
                    atkCountTime += checkCountTime;
                    if (atkCountTime > atkTime)
                    {
                        SetAtkRotation(dir);
                        Attack(md.mCfg.skillID);
                        atkCountTime = 0;
                    }
                    else
                    {
                        Idle();
                    }
                }
                checkCountTime = 0;
                checkTime = PETools.RDInt(1, 10) * 1.0f / 10;
            }
        }
    }

    public override Vector2 CalcTargetDir()
    {
        EntityPlayer entityPlayer = battleMgr.entitySelfPlayer;
        if(entityPlayer == null || entityPlayer.currentAniState == AniState.Die)
        {
            runAI = false;
            return Vector2.zero;
        }
        else
        {
            Vector3 target = entityPlayer.GetPos();
            Vector3 self = GetPos();
            return new Vector2(target.x - self.x, target.z - self.z).normalized;
        }
    }

    public bool InAtkRange()
    {
        EntityPlayer entityPlayer = battleMgr.entitySelfPlayer;
        if(entityPlayer == null || entityPlayer.currentAniState == AniState.Die)
        {
            runAI = false;
            return false;
        }
        else
        {
            Vector3 target = entityPlayer.GetPos();
            Vector3 self = GetPos();
            target.y = 0;
            self.y = 0;
            float dis = Vector3.Distance(target, self);
            if(dis <= md.mCfg.atkDis)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    #endregion

    public override bool GetBreakState()
    {
        if (md.mCfg.isStop)
        {
            if(curtSkillCfg != null)
            {
                return curtSkillCfg.isBreak;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public override void SetHP(int oldHp, int newHp)
    {
        if(md.mCfg.mType == MonsterType.Boss)
        {
            BattleSys.Instance.playerWnd.SetBossHPBarVal(oldHp,newHp,Props.hp);
        }
        else if(md.mCfg.mType == MonsterType.Normal)
        {
            base.SetHP(oldHp, newHp);
        }
    }
}
