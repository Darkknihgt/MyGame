/****************************************************
    文件：EntityBase.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/25 12:58:7
	功能：实体基类
*****************************************************/



using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase
{
    public AniState currentAniState = AniState.None;
    public StateMgr stateMgr = null;//问题，为什么初始化各种状态不放到实体数据类中进行？
    public Controller controller = null;
    public SkillMgr skillMgr = null;
    public BattleMgr battleMgr = null;

    public bool canControl = true;
    public bool canRlsSkill = true;

    public EntityType entityType = EntityType.None;
    public EntityState entityState = EntityState.None;

    //实现攻击联机
    public Queue<int> comboQue = new Queue<int>();
    public int nextSkillID = 0;
    public SkillConfigs curtSkillCfg;

    //技能移动回调函数返回值列表
    public List<int> SkillMoveCBLst = new List<int>();
    //技能伤害回调函数返回值列表
    public List<int> SkillActionCBLst = new List<int>();

    public int skEndCB = -1;

    private string name;

    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }
    #region 属性
    private BattleProps props;
    public BattleProps Props
    {
        get
        {
            return props;
        }

        protected set
        {
            props = value;
        }
    }

    private int hp;
    public int HP
    {
        get
        {
            return hp;
        }

        set
        {
            //PECommon.Log("hp change " + hp + " to " + value);
            SetHP(HP, value);
            hp = value;
        }
    }


    #endregion

    #region 处理战斗属性计算逻辑
    public virtual void SetBattleProps(BattleProps props)
    {
        HP = props.hp;
        Props = props;
    }
    #endregion
    /// <summary>
    /// 处理实体状态的函数
    /// </summary>

    public void Born()
    {
        stateMgr.ChangeStatus(this, AniState.Born);

    }

    public void Move()
    {
        stateMgr.ChangeStatus(this, AniState.Move);

    }

    public void Idle()
    {
        stateMgr.ChangeStatus(this, AniState.Idle);

    }

    public void Hit()
    {
        stateMgr.ChangeStatus(this, AniState.Hit);

    }

    public void Die()
    {
        stateMgr.ChangeStatus(this, AniState.Die);

    }

    public void Attack(int skillID)
    {
        stateMgr.ChangeStatus(this, AniState.Attack, skillID);
    }

    #region 转接方法
    /// <summary>
    /// 封装了角色控制器中的SetBlend方法
    /// </summary>
    /// <param name="blend"></param>

    public virtual void SetDir(Vector2 vector)
    {
        if (controller != null)
        {
            controller.Dir = vector;
        }
    }

    public virtual void SkillAttack(EntityBase entity, int skillID)
    {
        skillMgr.SkillAttack(entity, skillID);
    }
    public virtual void SetBlend(float blend)
    {
        if (controller != null)
        {
            controller.SetBlend(blend);
        }

    }

    public void SetCtrl(Controller ctrl)
    {
        controller = ctrl;
    }

    public virtual void SetAction(int act)
    {
        if (controller != null)
        {
            controller.SetAction(act);
        }
    }

    public virtual void SetFx(string name, float destroyTime)
    {
        if (controller != null)
        {
            controller.SetFx(name, destroyTime);
        }
    }
    public virtual void SetAtkRotation(Vector2 Dir, bool offset = false)
    {
        if (controller != null)
        {
            if (offset)
            {
                controller.SetAtkRotation(Dir);

            }
            else
            {
                controller.SetAtkRotationLocal(Dir);
            }
        }
    }


    public virtual void SkillMoveState(bool state, float speed = 0f)
    {
        if (controller != null)
        {
            controller.SetSkillMoveState(state, speed);
        }
    }


    public virtual void AttackEffect(int skillID)
    {
        skillMgr.AttackEffect(this, skillID);
    }



    /// <summary>
    /// 来自ItemEntityHp类的转接方法
    /// </summary>
    /// <returns></returns>
    public virtual void SetDodge()
    {
        if (controller != null)
        {
            GameRoot.Instance.dynamicWnd.SetDodge(Name);
        }
    }

    public virtual void SetCritical(int critical)
    {
        if (controller != null)
        {
            GameRoot.Instance.dynamicWnd.SetCritical(Name, critical);
        }
    }

    public virtual void SetHurt(int hurt)
    {
        if (controller != null)
        {
            GameRoot.Instance.dynamicWnd.SetHurt(Name, hurt);
        }
    }

    public virtual void SetHP(int oldHp, int newHp)
    {
        if (controller != null)
            GameRoot.Instance.dynamicWnd.SetHp(Name, oldHp, newHp);
    }



    #endregion

    public virtual Vector2 GetInputDir()
    {
        return Vector2.zero;
    }

    public virtual Vector3 GetPos()
    {
        return controller.transform.position;
    }

    public virtual Transform GetTrans()
    {
        return controller.transform;
    }

    public virtual Controller GetEntityCtrl()
    {
        return controller;
    }

    public virtual void SetActive(bool active = true)
    {
        if (controller != null)
        {
            controller.gameObject.SetActive(active);
        }
    }

    public virtual Vector2 CalcTargetDir()
    {
        return Vector2.zero;
    }

    public virtual bool GetBreakState()
    {
        return true;
    }

    public AudioSource GetAudio()
    {
        return controller.GetComponent<AudioSource>();
    }

    public CharacterController GetCC()
    {
        return controller.GetComponent<CharacterController>();
    }

    public virtual void TickAILogic() { }

    /// <summary>
    /// 处理实体退出攻击状态时的设定
    /// </summary>
    public void ExitCurtSkill()
    {
        canControl = true;

        if (curtSkillCfg != null)
        {

            if (!curtSkillCfg.isBreak)
            {
                entityState = EntityState.None;
            }
            if (curtSkillCfg.isCombo == true)
            {
                if (comboQue.Count > 0)
                {
                    nextSkillID = comboQue.Dequeue();
                }
                else
                {
                    nextSkillID = 0;
                }

            }
            curtSkillCfg = null;
        }
        SetAction(Constants.ActionDefault);
    }

    public void RemoveCB(int tid)
    {
        int index = -1;
        for (int i = 0; i < SkillMoveCBLst.Count; i++)
        {
            if (SkillMoveCBLst[i] == tid)
            {
                index = i;
                break;
            }
            if (index != -1)
            {
                SkillMoveCBLst.RemoveAt(index);
            }
        }
    }

    public void RemoveActionCB(int tid)
    {
        int index = -1;
        for (int i = 0; i < SkillActionCBLst.Count; i++)
        {
            if (SkillActionCBLst[i] == tid)
            {
                index = i;
                break;
            }
            if (index != -1)
            {
                SkillActionCBLst.RemoveAt(index);
            }
        }
    }

    public void RmvSkillCB()
    {
        SetDir(Vector2.zero);
        //解决受击后会处于技能移动状态的bug
        SkillMoveState(false);

        for (int i = 0; i < SkillMoveCBLst.Count; i++)
        {
            int tid = SkillMoveCBLst[i];
            TimerSvc.Instance.DelTask(tid);
        }
        for (int i = 0; i < SkillActionCBLst.Count; i++)
        {
            int tid = SkillActionCBLst[i];
            TimerSvc.Instance.DelTask(tid);
        }

        //攻击被中断，删除进入idle的定时回调
        if (skEndCB != -1)
        {
            TimerSvc.Instance.DelTask(skEndCB);
            skEndCB = -1;
        }
        SkillMoveCBLst.Clear();
        SkillActionCBLst.Clear();

        //清空连招
        if (nextSkillID != 0 ||comboQue.Count > 0)
        {
            nextSkillID = 0;
            comboQue.Clear();

            battleMgr.lastAtkTime = 0;
            battleMgr.comboIndex = 0;
        }
    }
}

