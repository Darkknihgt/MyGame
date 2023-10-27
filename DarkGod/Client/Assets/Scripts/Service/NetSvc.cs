/****************************************************
    文件：NetSvc.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/17 18:39:15
	功能：网络服务
*****************************************************/

using PENet;
using PEProtocol;
using System.Collections.Generic;
using UnityEngine;

public class NetSvc : MonoBehaviour 
{
    public static NetSvc Instance;

    PESocket<ClientSession, GameMsg> client = null;
    private Queue<GameMsg> msgQue = new Queue<GameMsg>();
    private static readonly string obj = "lock";

    public void InitNetSvc()
    {
        Instance = this;
        client = new PESocket<ClientSession, GameMsg>();

        //修改消息接口
        client.SetLog(true, (string msg, int lv) => {
            switch (lv)
            {
                case 0:
                    msg = "Log" + msg;
                    Debug.Log(msg);
                    break;

                case 1:
                    msg = "Warn" + msg;
                    Debug.LogWarning(msg);
                    break;

                case 2:
                    msg = "Error" + msg;
                    Debug.LogError(msg);
                    break;

                case 3:
                    msg = "Info" + msg;
                    Debug.Log(msg);
                    break;
            }

        });

        client.StartAsClient(SevCfg.srvIP, SevCfg.srvPort);

        PECommon.Log("Init NetSvc...");
    }

    public void SendMsg(GameMsg msg)
    {
        if(client.session != null)
        {
            client.session.SendMsg(msg);
        }
        else
        {
            GameRoot.AddTips("服务器未连接");
            InitNetSvc();
        }
    }

    /// <summary>
    /// 将传入的信息收纳到队列中
    /// </summary>
    public void AddNetPkg(GameMsg msg)
    {
        lock (obj)
        {
            msgQue.Enqueue(msg);
        }
    }

    /// <summary>
    /// 对Msg进行分门别类
    /// </summary>
    /// <param name="msg"></param>
    public void ProcessMsg(GameMsg msg)
    {
        if(msg.err != (int)ErrorCode.None)
        {
            switch ((ErrorCode)msg.err)
            {
                case ErrorCode.ServerDataError:
                    PECommon.Log("数据库异常", LogType.Error);
                    GameRoot.AddTips("客户端数据异常");
                    break;

                case ErrorCode.AcctIsOnline:
                    GameRoot.AddTips("账号已上线");
                    break;

                case ErrorCode.WrongPass:
                    GameRoot.AddTips("账号或密码有错");
                    break;

                case ErrorCode.NameIsExist:
                    GameRoot.AddTips("名字已存在");
                    break;

                case ErrorCode.UpdateDBError:
                    PECommon.Log("数据库更新异常", LogType.Error);
                    GameRoot.AddTips("网络异常");
                    break;

                case ErrorCode.ClentDataError:
                    PECommon.Log("客户端数据异常", LogType.Error);
                    //GameRoot.AddTips("网络异常");
                    break;

                case ErrorCode.LackLevel:
                    //PECommon.Log("等级不足", LogType.Error);
                    GameRoot.AddTips("角色等级不够");
                    break;

                case ErrorCode.LackCoin:
                    //PECommon.Log("金币不足", LogType.Error);
                    GameRoot.AddTips("金币不够");
                    break;

                case ErrorCode.LackCrystal:
                    //PECommon.Log("水晶不足", LogType.Error);
                    GameRoot.AddTips("水晶不够");
                    break;

                case ErrorCode.LackDiamond:
                    GameRoot.AddTips("钻石不够");
                    break;
            }
            return;
        }
        else
        {
            //判断cmd
            switch ((CMD)msg.cmd)
            {
                //处理登陆返回
                case CMD.RspLogin:
                    LoginSys.Instance.RspLogin(msg);
                    break;
                //处理名字信息更改
                case CMD.RspRename:
                    LoginSys.Instance.RspRename(msg);
                    break;
                case CMD.RspGuide:
                    MainCitySys.Instance.RspGuide(msg);
                    break;
                case CMD.RspStrong:
                    MainCitySys.Instance.RspStrong(msg);
                    break;
                case CMD.PshChat:
                    MainCitySys.Instance.PshChat(msg);
                    break;
                case CMD.RspBuy:
                    MainCitySys.Instance.RspBuy(msg);
                    break;
                case CMD.PshPower:
                    MainCitySys.Instance.PshPower(msg);
                    break;
                case CMD.RspTaskReward:
                    MainCitySys.Instance.RspTaskReward(msg);
                    break;
                case CMD.PshTaskPrgs:
                    MainCitySys.Instance.PshTaskPrgs(msg);
                    break;
                case CMD.RspFBFight:
                    FubenSys.Instance.RspFBFight(msg);
                    break;
                case CMD.RspFBFightEnd:
                    BattleSys.Instance.RspFightEnd(msg);
                    break;
            }
        }
    }

    /// <summary>
    /// 处理出队列的Msg
    /// </summary>
    private void Update()
    {
        if(msgQue.Count > 0)
        {
            lock (obj)
            {
                ProcessMsg(msgQue.Dequeue());
            }
        }
    }
}