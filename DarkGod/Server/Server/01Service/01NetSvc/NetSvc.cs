/****************************************************
    文件：NetSvc.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/15 23:5:29
	功能：网络服务
*****************************************************/



using PENet;
using PEProtocol;
using System.Collections.Generic;

//会话和消息打包
public class MsgPack
{
    public ServerSession srvSession;
    public GameMsg gameMsg;
    public MsgPack(ServerSession session, GameMsg msg)
    {
        srvSession = session;
        gameMsg = msg;
    }
}

public class NetSvc
{
    private static NetSvc instance = null;

    private static readonly string obj = "lock";

    /// <summary>
    /// singleton
    /// </summary>
    public static NetSvc Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new NetSvc();
            }
            return instance;
        }
    }

    public void InitNetSvc()
    {
        PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
        server.StartAsServer(SevCfg.srvIP, SevCfg.srvPort);

        PECommon.Log("NetSvc Init Done.");
    }

    //对从客户端收到的消息进行接收
    private Queue<MsgPack> msgPackQue = new Queue<MsgPack>();
    public void AddMsgQue(MsgPack msgPack)
    {
        lock (obj)
        {
            msgPackQue.Enqueue(msgPack);
        }
    }

    public void Update()
    {
        if(msgPackQue.Count > 0)
        {
            //PECommon.Log("Que Count: " + msgPackQue.Count);
            lock (obj)
            {
                HandOutMsg(msgPackQue.Dequeue());
            }
        }
    }

    public void HandOutMsg(MsgPack msgPack)
    {
        switch ((CMD)msgPack.gameMsg.cmd)
        {
            case CMD.ReqLogin:
                LoginSys.Instance.ReqLogin(msgPack);
                break;

            case CMD.ReqRename:
                LoginSys.Instance.ReqRename(msgPack);
                break;

            case CMD.ReqGuide:
                GuideSys.Instance.ReqGuide(msgPack);
                break;

            case CMD.ReqStrong:
                StrongSys.Instance.ReqStrong(msgPack);
                break;

            case CMD.SndChat:
                ChatSys.Instance.SndChat(msgPack);
                break;

            case CMD.ReqBuy:
                BuySys.Instance.ReqBuy(msgPack);
                break;
            case CMD.ReqTaskReward:
                taskSys.Instance.ReqTask(msgPack);
                break;
            case CMD.ReqFBFight:
                FubenSys.Instance.ReqFBFight(msgPack);
                break;
            case CMD.ReqFBFightEnd:
                FubenSys.Instance.ReqFBFightEnd(msgPack);
                break;
        }
    }
}

