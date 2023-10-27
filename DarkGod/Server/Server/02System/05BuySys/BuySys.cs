/****************************************************
    文件：BuySys.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/19 23:5:29
	功能：购买交易系统
*****************************************************/

using PEProtocol;
public class BuySys
{
    private static BuySys instance = null;
    public static BuySys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BuySys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;

    public void InitBuySys()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("BuySys Init Done");
    }

    public void ReqBuy(MsgPack pack)
    {
        ReqBuy data = pack.gameMsg.reqBuy;
        GameMsg msg = new GameMsg()
        {
            cmd = (int)CMD.RspBuy,
        };

        //获取玩家信息
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.srvSession);
        PshTaskPrgs pshTaskPrgs = null;
        //对玩家信息进行操作
        if (data.cost > pd.diamond)
        {
            msg.err = (int)ErrorCode.LackDiamond;
        }
        else
        {
            pd.diamond -= data.cost;
            
            switch (data.type)
            {
                case 0:
                    pd.power += 100;

                    #region 任务奖励进度更新
                    pshTaskPrgs = taskSys.Instance.GetTaskPrgs(pd, 4);
                    #endregion
                    break;
                case 1:
                    pd.coin += 1000;

                    #region 任务奖励进度更新
                    pshTaskPrgs = taskSys.Instance.GetTaskPrgs(pd, 5);
                    #endregion
                    break;
            }
        }
        //更新数据库数据
        if (!cacheSvc.UpdatePlayerData(pd.id, pd))
        {
            msg.err = (int)ErrorCode.UpdateDBError;
        }
        else
        {
            //成功更新数据库后，返回消息
            RspBuy rspBuy = new RspBuy
            {
                type = data.type,
                diamond = pd.diamond,
                coin = pd.coin,
                power = pd.power,
            };
            msg.rspBuy = rspBuy;
            msg.pshTaskPrgs = pshTaskPrgs;
            pack.srvSession.SendMsg(msg);
        }
    }
}

