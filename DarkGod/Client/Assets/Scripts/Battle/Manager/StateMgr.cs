/****************************************************
    文件：StateMgr.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/24 22:6:30
	功能：状态管理器
*****************************************************/
using System.Collections.Generic;
using UnityEngine;

public class StateMgr : MonoBehaviour
{
    //状态机，存储状态
    public Dictionary<AniState, IState> fsm = new Dictionary<AniState, IState>();

    public void Init()
    {
        //往状态机里添加状态
        fsm.Add(AniState.Born, new StateBorn());
        fsm.Add(AniState.Idle, new StateIdle());
        fsm.Add(AniState.Move, new StateMove());
        fsm.Add(AniState.Attack, new StateAttack());
        fsm.Add(AniState.Hit, new StateHit());
        fsm.Add(AniState.Die, new StateDie());

        PECommon.Log("Init stateMgr");
    }

    public void ChangeStatus(EntityBase entity,AniState targetState, params object[] args)
    {
        //如果当前的状态和目标状态相同，则无需转换
        if(entity.currentAniState == targetState)
        {
            return;
        }

        if (fsm.ContainsKey(targetState))
        {
            //离开当前状态
            if(entity.currentAniState != AniState.None)
            {
            fsm[entity.currentAniState].Exit(entity,args);
            }
            //进入目标状态
            fsm[targetState].Enter(entity,args);
            //持续目标状态
            fsm[targetState].Process(entity,args);
        }
    }
}

