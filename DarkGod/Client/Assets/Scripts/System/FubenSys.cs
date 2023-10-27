/****************************************************
    文件：FubenSys.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/23 21:52:44
	功能：副本系统
*****************************************************/

using System;
using System.Collections.Generic;
using PEProtocol;

public class FubenSys :SystemRoot
{
    public static FubenSys Instance = null;

    public BattleSys battleSys = null;
    public override void InitSys()
    {
        Instance = this;
        battleSys = BattleSys.Instance;
        base.InitSys();

        PECommon.Log("FubenSys Init...");
    }

    #region 字段
    public FubenWnd fubenWnd;

    #endregion

   

    #region 
    public void RspFBFight(GameMsg msg)
    {
        RspFBFight rspFBFight = msg.rspFBFight;

        GameRoot.Instance.SetPlayerDataByFBStart(msg.rspFBFight);
        MainCitySys.Instance.mainCityWnd.SetWndState(false);
        SetFubenWndState(false);

        battleSys.StartBattle(rspFBFight.fbid);
    }
    #endregion

    #region fubenWnd
    public void SetFubenWndState(bool isActive = true)
    {
        fubenWnd.SetWndState(isActive);
    }

    #endregion
}

