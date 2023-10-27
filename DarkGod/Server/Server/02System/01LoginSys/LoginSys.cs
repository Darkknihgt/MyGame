/****************************************************
    文件：LoginSys.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/15 23:5:29
	功能：登陆业务系统
*****************************************************/


using PENet;
using PEProtocol;

public class LoginSys
{
    private static LoginSys instance = null;

    /// <summary>
    /// singleton
    /// </summary>
    public static LoginSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LoginSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private TimerSvc timerSvc = null;

    public void InitLoginSys()
    {
        cacheSvc = CacheSvc.Instance;
        timerSvc = TimerSvc.Instance;
        PECommon.Log("LoginSys Init Done.");
    }

    public void ReqLogin(MsgPack msgPack)
    {
        ReqLogin data = msgPack.gameMsg.reqLogin;
        //当前账号时候上线
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspLogin,

        };
        if (cacheSvc.IsAcctOnLine(data.acct))
        {
            //已上线：返回错误信息
            msg.err = (int)(ErrorCode.AcctIsOnline);
        }
        else
        {
            //未上线：
            //账号是否存在
            PlayerData pd = cacheSvc.GetPlayerData(data.acct, data.pass);
            if (pd == null)
            {
                //存在，密码错误
                msg.err = (int)ErrorCode.WrongPass;
            }
            else
            {
                //对玩家的离线时间进行判定
                long time = pd.time;
                int power = pd.power;
                if (power < PECommon.GetPowerLimit(pd.lv)) //只有当玩家能量不足时才进行判定
                {
                    long nowTime = timerSvc.GetNowTime();
                    int addPower = (int)((nowTime - time) / (1000 * 60 *PECommon.PowerAddSpace));
                    if (addPower > 0)
                    {
                        pd.power += addPower;
                        if (pd.power > PECommon.GetPowerLimit(pd.lv))
                        {
                            pd.power = PECommon.GetPowerLimit(pd.lv);
                        }
                    }
                }
                //更新数据库数据
                if (power != pd.power)
                {
                    cacheSvc.UpdatePlayerData(pd.id, pd);
                }


                msg.rspLogin = new RspLogin
                {
                    playerData = pd
                };

                //缓存账号//不存在，创建默认的账号和密码
                cacheSvc.AcctOnline(data.acct, msgPack.srvSession, pd);
            }
        }



        //回应客户端
        msgPack.srvSession.SendMsg(msg);
    }

    public void ReqRename(MsgPack pack)
    {
        ReqRename reqRename = pack.gameMsg.reqRename;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspRename,
        };

        //判断名字是否存在
        if (cacheSvc.IsNameExist(reqRename))
        {
            //当名字存在数据库中
            msg.err = (int)ErrorCode.NameIsExist;
        }
        else
        {
            //不存在名字
            PlayerData playerdata = null;
            playerdata = cacheSvc.GetPlayerDataBySession(pack.srvSession);
            //缓存层修改
            playerdata.name = reqRename.name;
            //数据层修改
            if (!cacheSvc.UpdatePlayerData(playerdata.id, playerdata))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                msg.rspRename = new RspRename
                {
                    name = reqRename.name,
                };
            }
        }
        pack.srvSession.SendMsg(msg);
    }

    //下线触发
    public void ClearOfflineData(ServerSession session)
    {
        PlayerData pd = cacheSvc.GetPlayerDataBySession(session);

        if(pd != null)
        {
            pd.time = timerSvc.GetNowTime();
            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                PECommon.Log("Updata offline time error", LogType.Error);
            }
        }    
        cacheSvc.AcctOffLine(session);
    }
}

