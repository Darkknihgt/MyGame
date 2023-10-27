/****************************************************
    文件：ServerSession.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/15 23:5:29
	功能：网络会话链接
*****************************************************/



//用来和客户端建立连接
using PENet;
using PEProtocol;
using Server;

public class ServerSession : PESession<GameMsg>
{
    public int sessionID = 0;
    protected override void OnConnected()
    {
        sessionID = ServerRoot.Instance.GetSessionID();
        PECommon.Log("SessionID: "+sessionID + " Client Connect");
       
    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("SessionID: " + sessionID + " RcvPack CMD" + ((CMD)msg.cmd).ToString());

        NetSvc.Instance.AddMsgQue(new MsgPack(this,msg));
    }

    protected override void OnDisConnected()
    {
        LoginSys.Instance.ClearOfflineData(this);
        PECommon.Log("SessionID: " + sessionID + " Client DisConnect");
    }
}

