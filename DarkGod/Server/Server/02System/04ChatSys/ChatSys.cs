/****************************************************
    文件：ChatSys.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/118 23:5:29
	功能：聊天对话系统
*****************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PENet;
using PEProtocol;


public class ChatSys
{
    private static ChatSys instance = null;
    public static ChatSys Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new ChatSys { };
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;

    public void InitChatSys()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("ChatSys Init Done");
    }

    public void SndChat(MsgPack pack)
    {
        //收到消息，先对数据进行存储
        SndChat data = pack.gameMsg.sndChat;
        //获取玩家数据
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.srvSession);

        //设置广播信息内容
        GameMsg msg = new GameMsg
        {
            cmd =(int)CMD.PshChat,
            pshChat = new PshChat
            {
                name = pd.name,
                chatTxt = data.chatTxt,
            }
        };

        #region 任务奖励进度更新
        taskSys.Instance.CalcTaskPrgs(pd, 6);
        #endregion

        //广播所有玩家
        List<ServerSession> lst = cacheSvc.GetPOnlineServerSessions();

        //优化操作，将传递的信息提前转化为二进制数据
        Byte[] bytes = PENet.PETool.PackNetMsg(msg);
        for(int i = 0; i < lst.Count; i++)
        {
            lst[i].SendMsg(bytes);
        }

    }
}

