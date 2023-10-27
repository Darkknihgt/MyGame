/****************************************************
    文件：GameRoot.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/11 23:3:34
	功能：游戏启动，所有系统初始化
*****************************************************/

using PEProtocol;
using UnityEngine;

public class GameRoot : MonoBehaviour 
{
    public static GameRoot Instance = null;
    //调用loadingwnd
    public LoadingWnd loadingWnd;
    //调用dynamicWnd
    public DynamicWnd dynamicWnd;
    //存储玩家信息
    private PlayerData playerData = null;
    public PlayerData PlayerData
    {
        get
        {
            return playerData;
        }
    }


    private void Start()
    {
        Instance = this;
        //不能销毁此脚本
        DontDestroyOnLoad(this);

        //告诉日志游戏启动
        PECommon.Log("game start...");

        ClearUIRoot();
        Init();
    }

    
    public void Init()
    {

        //获取服务模块，并初始化
        NetSvc netSvc = GetComponent<NetSvc>();
        netSvc.InitNetSvc();
        ResSvc res = GetComponent<ResSvc>();
        res.InitSvc();
        AudioSvc auSvc = GetComponent<AudioSvc>();
        auSvc.InitAudioSvc();
        TimerSvc timeSvc = GetComponent<TimerSvc>();
        timeSvc.InitTimerSvc();

        //获取业务模块，并初始化
        LoginSys login = GetComponent<LoginSys>();
        login.InitSys();
        MainCitySys mainCity = GetComponent<MainCitySys>();
        mainCity.InitSys();
        BattleSys battleSys = GetComponent<BattleSys>();
        battleSys.InitSys();
        FubenSys fubenSys = GetComponent<FubenSys>();
        fubenSys.InitSys();

        dynamicWnd.SetWndState(true);  //应为其中的某个函数方法要调用ResSvc服务块，所以放在这里

        //进入登陆场景并加载相应UI
        login.EnterLogin();

        //计时器测试
        //TimerSvc.Instance.AddTimeTask((int tid) => {
        //    PECommon.Log("test#test#test#test#test#test#");
        //}, 10000);


    }
    /// <summary>
    /// 提示输入API
    /// </summary>
    /// <param name="tips"></param>
    public static void AddTips(string tips)
    {
        Instance.dynamicWnd.AddTips(tips);
    }

    private void ClearUIRoot()
    {
        Transform canvas = transform.Find("Canvas");
        for(int i = 0;i < canvas.childCount; i++)
        {
            canvas.GetChild(i).gameObject.SetActive(false);
        }
        
    }


    public void SetPlayerData(GameMsg msg)
    {
        playerData = msg.rspLogin.playerData;
    }

    public void SetPlayerName(string name)
    {
        playerData.name = name;
    }

    public void SetPlayerDataByGuide(RspGuide data)
    {
        playerData.coin = data.coin;
        playerData.exp = data.exp;
        playerData.lv = data.lv;
        playerData.guideid = data.nextguideid;
        
    }

    /// <summary>
    /// 对强化后服务端传来的数据处理
    /// </summary>
    /// <param name="data"></param>
    public void SetPlayerDataByStrong(RspStrong data)
    {
        playerData.coin = data.coin;
        playerData.hp = data.hp;
        playerData.ad = data.ad;
        playerData.ap = data.ap;
        playerData.addef = data.addef;
        playerData.apdef = data.apdef;
        playerData.crystal = data.crystal;

        playerData.strongArr = data.strongArr;
    }

    public void SetPlayerDataByBuy(RspBuy data)
    {
        playerData.coin = data.coin;
        playerData.diamond = data.diamond;
        playerData.power = data.power;
    }

    public void SetPlayerDataByPower(PshPower data)
    {
        playerData.power = data.power;
    }

    public void SetPlayerDataByTaskReward(RspTaskReward data)
    {
        playerData.coin = data.coin;
        playerData.lv = data.lv;
        playerData.exp = data.exp;
        playerData.taskArr = data.taskArr;
    }

    public void SetPlayerDataByTaskPrgs(PshTaskPrgs data)
    {
        playerData.taskArr = data.taskArr;
    }

    public void SetPlayerDataByFBStart(RspFBFight data)
    {
        playerData.power = data.power;
    }

    public void SetPlayerDataByFBEnd(RspFBFightEnd data)
    {
        PlayerData.coin = data.coin;
        playerData.lv = data.lv;
        playerData.exp = data.exp;
        playerData.crystal = data.crystal;
        playerData.fuben = data.fubenPrg;
    }

}