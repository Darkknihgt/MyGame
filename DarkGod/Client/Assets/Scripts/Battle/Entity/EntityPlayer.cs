/****************************************************
    文件：EntityPlayer.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/24 22:6:30
	功能：玩家的逻辑实体
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public class EntityPlayer : EntityBase
{
    
    public EntityPlayer()
    {
        entityType = EntityType.Player;
    }

    public override Vector2 GetInputDir()
    {
        return BattleSys.Instance.GetInputDir();
    }

    public override Vector2 CalcTargetDir()
    {
        EntityMonster monster = FindClosedTarget();
        if(monster != null)
        {
            Vector3  target = monster.GetPos();
            Vector3 self = GetPos();
            Vector2 dir = new Vector2(target.x - self.x, target.z - self.z);
            return dir.normalized;
        }
        else
        {
            return Vector2.zero;
        }
    }

    private EntityMonster FindClosedTarget()
    {
        List<EntityMonster> lst = battleMgr.GetEntityMonster();
        if(lst == null || lst.Count == 0)
        {
            return null;
        }

        Vector3 self = GetPos();
        EntityMonster targetMonster = null;
        float dis = 0;
        for(int i = 0;i < lst.Count; i++)
        {
            Vector3 target = lst[i].GetPos();
            if(i == 0)
            {
                dis = Vector3.Distance(self, target);
                targetMonster = lst[0];
            }
            else
            {
                float calcDis = Vector3.Distance(self, target);
                if(dis > calcDis)
                {
                    dis = calcDis;
                    targetMonster = lst[i];
                }
            }
        }
        return targetMonster;
    }

    public override void SetHP(int oldHp, int newHp)
    {
        BattleSys.Instance.playerWnd.SetSelfHPBarBal(newHp);
    }

    public override void SetDodge()
    {
        GameRoot.Instance.dynamicWnd.SetSelfDodge();
    }
}

