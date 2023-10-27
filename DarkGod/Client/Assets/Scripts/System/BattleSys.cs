/****************************************************
    文件：BattleSys.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/24 21:49:14
	功能：战斗系统管理
*****************************************************/

using PEProtocol;
using UnityEngine;

public class BattleSys : SystemRoot 
{
    public static BattleSys Instance = null;

    public PlayerCtrlWnd playerWnd;
    public BattleMgr battleMgr;
    public BattleEndWnd battleEndWnd;

    private int fbid;
    private double startTime;

    public override void InitSys()
    {

        Instance = this;
        base.InitSys();

        
        PECommon.Log("Init BattleSys...");
    }

    public void StartBattle(int mapid)
    {
        fbid = mapid;
        GameObject go = new GameObject
        {
            name = "BattleRoot",          
        };
        go.transform.SetParent(GameRoot.Instance.transform);

        battleMgr = go.AddComponent<BattleMgr>();
        battleMgr.Init(mapid,() => {
            startTime = TimerSvc.Instance.GetNowTime();
        });


        //打开战斗窗口
        SetPlayerCtrlWnd(true);
    }

    public void EndBattle(bool isWin,int restHP)
    {
        //关闭控制界面，移除血条
        playerWnd.SetWndState(false);
        GameRoot.Instance.dynamicWnd.RemoveHPItemInfo();

        if (isWin)
        {
            double endTime = TimerSvc.Instance.GetNowTime();

            //发送结算战斗请求
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqFBFightEnd,
                reqFBFightEnd = new ReqFBFightEnd
                {
                    win = isWin,
                    fbid = fbid,
                    resthp = restHP,
                    costTime = (int)(endTime - startTime),
                },
            };
            netSvc.SendMsg(msg);
        }
        else
        {
            SetBattleEndWnd(FBEndType.Lose);
        }
    }

    #region 战斗窗口激活与关闭
    public void SetPlayerCtrlWnd(bool isActive = true)
    {
        playerWnd.SetWndState(isActive);
    }
    #endregion

    #region 副本结束
    public void SetBattleEndWnd(FBEndType endType, bool isActive = true)
    {
        battleEndWnd.SetWndType(endType);
        battleEndWnd.SetWndState(isActive);
    }

    public void RspFightEnd(GameMsg msg)
    {
        RspFBFightEnd data = msg.rspFBFightEnd;
        GameRoot.Instance.SetPlayerDataByFBEnd(data);

        battleEndWnd.BattleEndData(data.fbid, data.costTime, data.resthp);
        SetBattleEndWnd(FBEndType.Win, true);
    }
    #endregion

    //public void ReqReleaseSkill(int id)
    //{
    //    battleMgr.ReqReleaseSkill(id);
    //}
    public Vector2 GetInputDir()
    {
        return playerWnd.GetInputDir();
    }

    public void DestroyBattle()
    {
        SetPlayerCtrlWnd(false);
        SetBattleEndWnd(FBEndType.None, false);
        GameRoot.Instance.dynamicWnd.RemoveHPItemInfo();
        Destroy(battleMgr.gameObject);
    }
}