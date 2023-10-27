/****************************************************
    文件：LoginSys.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/13 23:5:29
	功能：强化系统
*****************************************************/

using PENet;
using PEProtocol;
class StrongSys
{
    private static StrongSys instance = null;
    public static StrongSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new StrongSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;

    //对系统初始化
    public void InitStrongSys()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("StrongSys Init Done");
    }

    //对客户端传入的消息包进行处理
    public void ReqStrong(MsgPack pack)
    {
        ReqStrong data = pack.gameMsg.reqStrong; //获取包中信息

        GameMsg msg = new GameMsg  //创建回传信息，并设置回传代号
        {
            cmd = (int)CMD.RspStrong,
        };

        //因为要对数据库中的玩家数据进行处理
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.srvSession);
        //读取数据库中的配置文件信息
        int curtStartLv = pd.strongArr[data.pos];
        StrongConfigs nextSd = cfgSvc.GetStrongConfigsByPosAndStarlv(data.pos, curtStartLv + 1);

        //条件判断
        if(pd.lv < nextSd.minlv)
        {
            msg.err = (int)ErrorCode.LackLevel;
        }
        else if(pd.coin < nextSd.coin)
        {
            msg.err = (int)ErrorCode.LackCoin;
        }else if(pd.crystal < nextSd.crystal)
        {
            msg.err = (int)ErrorCode.LackCrystal;
        }
        
        else
        {
            #region 任务奖励进度更新
            taskSys.Instance.CalcTaskPrgs(pd, 3);
            #endregion

            //资源扣除
            pd.coin -= nextSd.coin;
            pd.crystal -= nextSd.crystal;
            pd.strongArr[data.pos] += 1;

            //属性增加
            pd.hp += nextSd.addhp;
            pd.ad += nextSd.addhurt;
            pd.ap += nextSd.addhurt;
            pd.addef += nextSd.adddef;
            pd.apdef += nextSd.adddef;
        }

        //更新数据库
        if (!cacheSvc.UpdatePlayerData(pd.id, pd))
        {
            msg.err = (int)ErrorCode.UpdateDBError;
        }
        else
        {
            msg.rspStrong = new RspStrong
            {
                coin = pd.coin,
                crystal = pd.crystal,
                hp = pd.hp,
                ad = pd.ad,
                addef=pd.addef,
                ap = pd.ap,
                apdef = pd.apdef,
                strongArr= pd.strongArr,
            };

            pack.srvSession.SendMsg(msg);
        }


    }
}

