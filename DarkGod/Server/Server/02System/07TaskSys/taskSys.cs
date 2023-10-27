/****************************************************
    文件：NetSvc.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/21 23:5:29
	功能：任务奖励系统
*****************************************************/

using PEProtocol;

public class taskSys
{
    private static taskSys instance = null;
    public static taskSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new taskSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private TimerSvc timerSvc = null;
    private CfgSvc cfgSvc = null;
    public void InitTaskSys()
    {
        cacheSvc = CacheSvc.Instance;
        timerSvc = TimerSvc.Instance;
        cfgSvc = CfgSvc.Instance;

        PECommon.Log("Init TaskSys Done");
    }

    public void ReqTask(MsgPack pack)
    {
        ReqTaskReward data = pack.gameMsg.reqTaskReward;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspTaskReward,
        };

        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.srvSession);
        TaskRewardCfg trCfg = cfgSvc.GetTaskRewardCfgByID(data.tid);
        TaskRewardData trData =CalcTaskRewardData(pd,data.tid);

        //对获得的数据进行判断和修改
        if (trData.prgs == trCfg.count && !trData.taked) //只有当任务进度满，并且还未领取状态
        {
            pd.coin += trCfg.coin;
            PECommon.CalcExp(pd, trCfg.exp);
            trData.taked = true;

            //更新任务的缓存数据
            CalcTaskArr(pd, trData);

            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                RspTaskReward rspTaskReward = new RspTaskReward
                {
                    coin = pd.coin,
                    exp = pd.exp,
                    lv = pd.lv,
                    taskArr = pd.taskArr,
                };
                msg.rspTaskReward = rspTaskReward;
                
            }
        }
        else
        {
            msg.err = (int)ErrorCode.ClentDataError;
        }
        pack.srvSession.SendMsg(msg);
    }

    /// <summary>
    /// 缓存数据中的玩家任务奖励进度数据的解析
    /// </summary>
    /// <param name="pd"></param>
    /// <param name="tid"></param>
    /// <returns></returns>
    public TaskRewardData CalcTaskRewardData(PlayerData pd, int tid)
    {
        TaskRewardData trd = null;
        for (int i = 0; i < pd.taskArr.Length; i++)
        {

            string[] stData = pd.taskArr[i].Split('|');
            if (int.Parse(stData[0]) == tid)
            {
                trd = new TaskRewardData
                {
                    id = int.Parse(stData[0]),
                    prgs = int.Parse(stData[1]),
                    taked = stData[2].Equals("1"),
                };
                break;
            }

        }
        return trd;
    }

    /// <summary>
    /// 查找更新后任务奖励数据在任务数据中的位置，并进行覆盖
    /// </summary>
    /// <param name="pd"></param>
    /// <param name="trd"></param>
    public void CalcTaskArr(PlayerData pd,TaskRewardData trd)
    {
        string result = trd.id + "|" + trd.prgs + "|" + (trd.taked?1:0);
        int index = -1;
        for(int i =0;i < pd.taskArr.Length; i++)
        {
            string[] taskInfo = pd.taskArr[i].Split('|');
            if(int.Parse(taskInfo[0]) == trd.id)
            {
                index = i;
                break;
            }
        }
        pd.taskArr[index] = result;
    }

    /// <summary>
    /// 处理更新任务奖励的进度
    /// </summary>
    /// <param name="pd"></param>
    /// <param name="tid"></param>
    public void CalcTaskPrgs(PlayerData pd,int tid)
    {
        TaskRewardCfg trCfg = cfgSvc.GetTaskRewardCfgByID(tid);
        TaskRewardData trData = CalcTaskRewardData(pd,tid);

        if(trData.prgs < trCfg.count)
        {
            trData.prgs += 1;
            //更新其数据
            CalcTaskArr(pd, trData);
            //ErrorCode errs = ErrorCode.None;
            //if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            //{
            //    errs = ErrorCode.UpdateDBError;
            //}
            //在服务端更新完数据后将其发送给客户端进行更新
            ServerSession session = cacheSvc.GetOnlineServersessionByID(pd.id);
            session.SendMsg(new GameMsg { 
                cmd = (int)CMD.PshTaskPrgs,
                //err =(int)errs,
                pshTaskPrgs = new PshTaskPrgs
                {
                    taskArr = pd.taskArr,
                },
            });
        }
    }

    /// <summary>
    /// 优化网络处理，将多个包并包
    /// </summary>
    /// <param name="pd"></param>
    /// <param name="tid"></param>
    public PshTaskPrgs GetTaskPrgs(PlayerData pd, int tid)
    {
        TaskRewardCfg trCfg = cfgSvc.GetTaskRewardCfgByID(tid);
        TaskRewardData trData = CalcTaskRewardData(pd, tid);

        if (trData.prgs < trCfg.count)
        {
            trData.prgs += 1;
            //更新其数据
            CalcTaskArr(pd, trData);

            //cacheSvc.UpdatePlayerData(pd.id, pd);

            return new PshTaskPrgs
            {
                taskArr = pd.taskArr,
            };

        }
        return null;
    }
}

