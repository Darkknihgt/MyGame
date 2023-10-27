/****************************************************
    文件：NetSvc.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/19 23:5:29
	功能：体力增加系统
*****************************************************/


using PEProtocol;
using System.Collections.Generic;

public class PowerSys
{
    private static PowerSys instance = null;
    public static PowerSys Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new PowerSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private TimerSvc timerSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        timerSvc = TimerSvc.Instance;

        TimerSvc.Instance.AddTimeTask(CalcPowerAdd, PECommon.PowerAddSpace, PETimeUnit.Minute, 0);

        PECommon.Log("PowerSys Init Done");
    }

    private void CalcPowerAdd(int tid)
    {
        //打印日志
        PECommon.Log("All Online Player Calc Power Incress...");

        GameMsg msg = new GameMsg()
        {
            cmd = (int)CMD.PshPower,
        };
        msg.pshPower = new PshPower();

        //所有在线玩家获得实时的体力增长推送数据
        Dictionary<ServerSession, PlayerData> onlineDic = cacheSvc.GetOnlineCache();

        foreach(var item in onlineDic)
        {
            //遍历每个在线玩家数据
            PlayerData pd = item.Value;
            ServerSession session = item.Key;

            //获取当前玩家的体力上限
            int powerMax = PECommon.GetPowerLimit(pd.lv);
            if(pd.power >= powerMax)
            {
                //说明该玩家体力值超过上限
                continue;
            }
            else
            {
                pd.power += PECommon.PowerAddCount;
                pd.time = timerSvc.GetNowTime();
                if(pd.power > powerMax)
                {
                    pd.power = powerMax;
                }
            }

            //更新数据库数据
            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                msg.pshPower.power = pd.power;
                session.SendMsg(msg);
            }
        }
    }
}

