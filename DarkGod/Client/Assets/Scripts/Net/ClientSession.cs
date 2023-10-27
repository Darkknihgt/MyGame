/****************************************************
    文件：ClientSession.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/17 18:52:17
	功能：clientSession 消息处理
*****************************************************/

using UnityEngine;
using PENet;
using PEProtocol;

public class ClientSession :  PESession<GameMsg>
{
    protected override void OnConnected()
    {
        GameRoot.AddTips("服务器成功连接");
        PECommon.Log("Connect TO Sever Success");
    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("svcPack Rsp : " +  ((CMD)msg.cmd).ToString());
        NetSvc.Instance.AddNetPkg(msg);
    }

    protected override void OnDisConnected()
    {
        GameRoot.AddTips("服务器断开连接");
        PECommon.Log("Server Disconnect");
    }
}