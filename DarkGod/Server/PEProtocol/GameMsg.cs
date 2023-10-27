/****************************************************
    文件：GameMsg.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/15 23:5:29
	功能：网络通信协议(服务端)
*****************************************************/

using System;
using PENet;

namespace PEProtocol
{
    [Serializable]
    public class GameMsg : PENet.PEMsg
    {
        public ReqLogin reqLogin;
        public RspLogin rspLogin;

        //起名
        public ReqRename reqRename;
        public RspRename rspRename;

        //导航任务
        public ReqGuide reqGuide;
        public RspGuide rspGuide;

        //强化
        public ReqStrong reqStrong;
        public RspStrong rspStrong;

        //对话
        public SndChat sndChat;
        public PshChat pshChat;

        //购买金币体力系统
        public ReqBuy reqBuy;
        public RspBuy rspBuy;

        //体力恢复系统
        public PshPower pshPower;

        //任务奖励
        public ReqTaskReward reqTaskReward;
        public RspTaskReward rspTaskReward;
        public PshTaskPrgs pshTaskPrgs;

        //副本战斗申请
        public ReqFBFight reqFBFight;
        public RspFBFight rspFBFight;

        //战斗结束
        public ReqFBFightEnd reqFBFightEnd;
        public RspFBFightEnd rspFBFightEnd;
    }

    #region 登陆相关
    [Serializable]
    public class ReqLogin
    {
        public string acct;
        public string pass;
    }

    [Serializable]
    public class RspLogin
    {
        public PlayerData playerData;
    }

    [Serializable]
    public class ReqRename
    {
        public string name;
    }
    [Serializable]
    public class RspRename
    {
        public string name;
    }
    #endregion


    #region 主城相关

    [Serializable]
    public class ReqGuide
    {
        public int guideid;
    }

    [Serializable]
    public class RspGuide
    {
        public int nextguideid;
        public int coin;
        public int lv;
        public int exp;
    }

    #endregion

    #region 强化数据

    [Serializable]
    public class ReqStrong
    {
        public int pos;
    }

    [Serializable]
    public class RspStrong
    {
        public int coin;
        public int crystal;
        public int hp;
        public int ad;
        public int ap;
        public int addef;
        public int apdef;
        public int[] strongArr;
    }

    #endregion

    #region 发送信息与广播信息
    [Serializable]
    public class SndChat
    {
        public string chatTxt;
    }

    [Serializable]
    public class PshChat
    {
        public string name;
        public string chatTxt;
    }
    #endregion

    #region 交易购物相关
    [Serializable]
    public class ReqBuy
    {
        public int type;
        public int cost;
    }

    [Serializable]
    public class RspBuy
    {
        public int diamond;
        public int coin;
        public int power;
        public int type;
    }
    #endregion

    #region 体力回复
    [Serializable]
    public class PshPower
    {
        public int power;
    }
    #endregion

    #region 任务奖励
    [Serializable]
    public class ReqTaskReward
    {
        public int tid;
    }

    [Serializable]
    public class RspTaskReward
    {
        public int coin;
        public int lv;
        public int exp;
        public string[] taskArr;
    }

    [Serializable]
    public class PshTaskPrgs
    {
        public string[] taskArr;
    }
    #endregion

    #region 副本战斗相关
    [Serializable]
    public class ReqFBFight
    {
        public int fbid;
    }

    [Serializable]
    public class RspFBFight
    {
        public int fbid;
        public int power;
    }

    [Serializable]
    public class ReqFBFightEnd
    {
        public bool win;
        public int fbid;
        public int resthp;
        public int costTime;
    }
    [Serializable]
    public class RspFBFightEnd
    {
        public bool win;
        public int fbid;
        public int resthp;
        public int costTime;

        //副本奖励
        public int coin;
        public int lv;
        public int exp;
        public int crystal;
        public int fubenPrg;

    }
    #endregion
    [Serializable]
    public class PlayerData
    {
        public int id;
        public string name;
        public int lv;
        public int exp;
        public int power;
        public int coin;
        public int diamond;
        public int crystal;

        public int hp;
        public int ad;
        public int ap;
        public int addef;
        public int apdef;
        public int dodge; //闪避
        public int pierce; //穿透率
        public int critical; //暴击率
        public int[] strongArr; //强化等级数组

        public int guideid;

        public long time;//离线时间判定

        public string[] taskArr;
        public int fuben;
        
    }


    public enum CMD
    {
        None = 0,
        //处理登陆相关
        ReqLogin =101,
        RspLogin = 102,
        //起名
        ReqRename =103,
        RspRename= 104,

        //主城相关 200
        ReqGuide = 201,
        RspGuide = 202,

        ReqStrong = 203,
        RspStrong = 204,

        SndChat = 205,
        PshChat = 206,

        ReqBuy = 207,
        RspBuy = 208,

        PshPower = 209,

        ReqTaskReward =210,
        RspTaskReward = 211,

        PshTaskPrgs = 212,

        //副本相关
        ReqFBFight =301,
        RspFBFight = 302,

        ReqFBFightEnd = 303,
        RspFBFightEnd = 304,
    }

    public enum ErrorCode
    {
        None = 0,

        AcctIsOnline,//账号已经上线
        WrongPass, //密码错误
        NameIsExist,//名字存在
        UpdateDBError,//更新数据库出错
        ServerDataError,
        ClentDataError,

        LackLevel,// 缺少等级
        LackCoin, // 缺少金币
        LackCrystal, //缺少水晶
        LackDiamond,
        LackPower,

    }

    //端口和IP配置
    public class SevCfg
    {
        public const string srvIP = "127.0.0.1";
        public const int srvPort = 17666;
    }
}
