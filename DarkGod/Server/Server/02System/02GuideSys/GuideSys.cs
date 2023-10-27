/****************************************************
    文件：LoginSys.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/15 23:5:29
	功能：引导任务系统
*****************************************************/

using PENet;
using PEProtocol;


public class GuideSys
{
    private static GuideSys instance = null;
    public static GuideSys Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GuideSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;

    public void InitGuideSys()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("GuideSys Init Done.");
    }

    public void ReqGuide(MsgPack msgpack)
    {
        ReqGuide data = msgpack.gameMsg.reqGuide;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspGuide,
        };
        PlayerData pd = cacheSvc.GetPlayerDataBySession(msgpack.srvSession);
        GuideConfigs gc = cfgSvc.GetGuideDataByID(data.guideid);
        //更新玩家引导任务
        if(pd.guideid == data.guideid)
        {
            #region 处理任务奖励完成的代码
            //判断该完成的任务是否为智者点拨
            if(data.guideid == 1001)
            {
                //更新任务奖励数据
                taskSys.Instance.CalcTaskPrgs(pd, 1);
            }


            #endregion

            //先更新玩家缓存中引导ID
            pd.guideid += 1;
            //更新玩家数据
            pd.coin += gc.coin;
            PECommon.CalcExp(pd, gc.exp);

            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                msg.rspGuide = new RspGuide
                {
                    nextguideid = pd.guideid,
                    lv = pd.lv,
                    exp = pd.exp,
                    coin = pd.coin,
                };
            }
        }
        else
        {
            msg.err = (int)ErrorCode.ServerDataError;
        }
        msgpack.srvSession.SendMsg(msg);
    }

    
}
