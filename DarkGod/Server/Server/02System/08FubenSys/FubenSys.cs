/****************************************************
    文件：FubenSys.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/24 23:5:29
	功能：购买交易系统
*****************************************************/

using PEProtocol;
using PENet;

public class FubenSys
{
    private static FubenSys instance = null;
    public static FubenSys Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new FubenSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;

    public void InitFubenSys()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("FubenSys Init Done");
    }

    public void ReqFBFight(MsgPack pack)
    {
        ReqFBFight data = pack.gameMsg.reqFBFight;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspFBFight,
        };

        //获取玩家数据与地图配置数据
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.srvSession);
        MapConfigs mapCfg = cfgSvc.GetMapCfg(data.fbid);
        int power = mapCfg.power;

        if(data.fbid > pd.fuben)
        {
            msg.err = (int)ErrorCode.ClentDataError;
        }else if(pd.power < power)
        {
            msg.err = (int)ErrorCode.LackPower;
        }
        else
        {
            pd.power -= power;
            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                msg.rspFBFight = new RspFBFight
                {
                    fbid = data.fbid,
                    power = pd.power
                };
            }
        }
        pack.srvSession.SendMsg(msg);
    }

    public void ReqFBFightEnd(MsgPack pack)
    {
        ReqFBFightEnd data = pack.gameMsg.reqFBFightEnd;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspFBFightEnd,
        };
        //校验战斗是否合法
        if (data.win)
        {
            if(data.costTime > 0 && data.resthp > 0)
            {
                //根据副本ID获取相应奖励
                MapConfigs rd = cfgSvc.GetMapCfg(data.fbid);
                PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.srvSession);

                //任务进度数据更新
                taskSys.Instance.CalcTaskPrgs(pd, 2);

                pd.coin += rd.coin;
                pd.crystal += rd.crystal;
                PECommon.CalcExp(pd, rd.exp);

                if(pd.fuben == data.fbid)
                {
                    pd.fuben += 1;
                }
                if(!cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else
                {
                    RspFBFightEnd rspFBFightEnd = new RspFBFightEnd
                    {
                        win = data.win,
                        fbid = data.fbid,
                        costTime = data.costTime,
                        resthp = data.resthp,

                        coin = pd.coin,
                        exp = pd.exp,
                        lv = pd.lv,
                        crystal = pd.crystal,
                        fubenPrg = pd.fuben,
                    };

                    msg.rspFBFightEnd = rspFBFightEnd;
                }
            }
        }
        else
        {
            msg.err =(int)ErrorCode.ClentDataError;
        }

        pack.srvSession.SendMsg(msg);
    }
}

