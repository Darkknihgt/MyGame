/****************************************************
    文件：MainCitySys.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/19 12:58:7
	功能：主城业务逻辑系统
*****************************************************/

using UnityEngine;
using UnityEngine.AI;
using PEProtocol;

public class MainCitySys : SystemRoot
{
    public static MainCitySys Instance = null;

    //任务信息
    public GuideConfigs guideTask;
    private Transform[] npcPosTrans;
    private NavMeshAgent navGuide;
    public MainCityWnd mainCityWnd;
    public CharInfoWnd charWnd;
    public GuideWnd guideWnd;
    public StrongWnd strongWnd;
    public ChatWnd chatWnd;
    public BuyWnd buyWnd;
    public TaskWnd taskWnd;

    //获取人物的控制
    private PlayerController playerCtrl;

    //获取人物展示相机
    private Transform ShowCamTrans;

    public override void InitSys()
    {
        Instance = this;
        base.InitSys();

        PECommon.Log("MainCitySys Init...");
        
    }

    
    public void EnterMainCity()
    {
        MapConfigs mapCfgs = resSvc.GetMapInfo(Constants.SceneMaincityID);
        resSvc.AsyncLoadScene(mapCfgs.sceneName, ()=> {
            PECommon.Log("Enter MainCity...");

            //TODO 加载主角
            LoadPlayerInfo(mapCfgs);
            //打开主城UI
            mainCityWnd.SetWndState(true);

            //切换声音接收器
            GameRoot.Instance.gameObject.GetComponent<AudioListener>().enabled = false;

            //播放主城背景音乐
            auSvc.PlayBGMusic(Constants.BGMainCity);

            //获取NPC位置信息
            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
            MapRoot mr = map.GetComponent<MapRoot>();
            npcPosTrans = mr.npcPosTrans;

            //TODO设置人物相机
            if(ShowCamTrans != null)
            {
                ShowCamTrans.gameObject.SetActive(false);
            }
        });
    }

    #region 进入副本系统业务逻辑
    public void EnterFubenSys()
    {
        FubenSys.Instance.SetFubenWndState(true);
    }
    #endregion

    public void LoadPlayerInfo(MapConfigs mapCfg)
    {
        GameObject player = resSvc.LoadPrefabs(PathDefine.AssassinCityPlayerPrefab, true);
        player.transform.position = mapCfg.playerBornPos;
        player.transform.localEulerAngles = mapCfg.playerBornRote;
        player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        Camera.main.transform.position = mapCfg.mainCamPos;
        Camera.main.transform.localEulerAngles = mapCfg.mainCamRote;

        //获取实例化角色身上的组件
        playerCtrl = player.GetComponent<PlayerController>();
        playerCtrl.Init();
        navGuide = playerCtrl.GetComponent<NavMeshAgent>();
    }

    public void SetMoveDir(Vector2 dir)
    {
        StopNavTask();
        if (dir == Vector2.zero)
        {
            playerCtrl.SetBlend(Constants.idleAnim);
        }
        else
        {
            playerCtrl.SetBlend(Constants.walkAnim);
        }
        playerCtrl.Dir = dir;
    }

    public void OpenInfoWnd()
    {
        StopNavTask();
        if (ShowCamTrans == null)
        {
            ShowCamTrans = GameObject.FindGameObjectWithTag("CharShowCam").transform;
        }
        //设置相机的相对位置
        ShowCamTrans.localPosition = playerCtrl.transform.position + playerCtrl.transform.forward * 3.8f + new Vector3(0, 1.2f, 0);
        ShowCamTrans.localEulerAngles = new Vector3(0, 180 + playerCtrl.transform.localEulerAngles.y, 0);
        ShowCamTrans.localScale = Vector3.one;
        ShowCamTrans.gameObject.SetActive(true);

        charWnd.SetWndState(true);
    }

    public void CloseInfoWnd()
    {
        charWnd.SetWndState(false);
        ShowCamTrans.gameObject.SetActive(false);
    }

    //记录人物调用前的旋转信息
    private float startRotate = 0;
    public void SetStartPos()
    {
        startRotate = playerCtrl.transform.localEulerAngles.y; 
    }

    public void SetPlayerRotate(float rotate)
    {
        playerCtrl.transform.localEulerAngles = new Vector3(0, startRotate + rotate, 0);
    }

    #region 导航相关
    private bool isNavGuide = false;
    public void RunTask(GuideConfigs gc)
    {
        if(gc != null)
        {
            guideTask = gc;
        }

        //解析任务数据
        navGuide.enabled = true;
        if (guideTask.npcID != -1)
        {
            //进行导航
            float dis = Vector3.Distance(playerCtrl.transform.position, npcPosTrans[guideTask.npcID].position);
            if(dis < 0.5f)
            {
                isNavGuide = false;
                navGuide.isStopped = true;
                playerCtrl.SetBlend(Constants.idleAnim);
                navGuide.enabled = false;
            }
            else //进行导航
            {
                isNavGuide = true;
                navGuide.enabled = true;
                navGuide.speed = Constants.playerSpeed;
                navGuide.SetDestination(npcPosTrans[guideTask.npcID].position);
                playerCtrl.SetBlend(Constants.walkAnim);
            }
        }
        else
        {
            OpenGuideWnd();
        }
    }

    private void Update()
    {
        if (isNavGuide)
        {
            IsArriveNavPos();
            playerCtrl.SetCam();            
        }
    }
    /// <summary>
    /// 是否到达目标位置
    /// </summary>
    private void IsArriveNavPos()
    {
        float dis = Vector3.Distance(playerCtrl.transform.position, npcPosTrans[guideTask.npcID].position);
        if (dis < 0.5f)
        {
            isNavGuide = false;
            navGuide.isStopped = true;
            playerCtrl.SetBlend(Constants.idleAnim);
            navGuide.enabled = false;

            OpenGuideWnd();
        }
    }
    /// <summary>
    /// 中断导航
    /// </summary>
    private void StopNavTask()
    {
        if (isNavGuide)
        {
            isNavGuide = false;

            navGuide.isStopped = true;
            navGuide.enabled = false;
            playerCtrl.SetBlend(Constants.idleAnim);
        }
    }

    /// <summary>
    /// 引导对话界面
    /// </summary>
    private void OpenGuideWnd()
    {
        //auSvc.PlayUIMusic(Constants.ClickUI);
        guideWnd.SetWndState(true);
    }


    #endregion
    /// <summary>
    /// 处理分包过来的数据
    /// </summary>
    /// <param name="msg"></param>
    public void RspGuide(GameMsg msg)
    {
        RspGuide data = msg.rspGuide;

        GameRoot.AddTips(Constants.Color("任务奖励 金币 +" + guideTask.coin + "经验" + guideTask.exp,TxtColor.Blue));

        switch (guideTask.actID)
        {
            case 0:
                //与智者对话
                break;
            case 1:
                //进入副本
                EnterFubenSys();
                break;
            case 2:
                //进入强化界面
                OpenStrongWnd();
                break;
            case 3:
                //进入体力购买界面
                OpenBuyWnd(0);
                break;
            case 4:
                //进入金币铸造
                OpenBuyWnd(1);
                break;
            case 5:
                //进入世界聊天
                OpenChatWnd();
                break;
        }

        GameRoot.Instance.SetPlayerDataByGuide(data);  //修改客户端玩家数据
        mainCityWnd.RefreshUI(); //更新UI界面
    }

    #region 处理强化

    public void RspStrong(GameMsg msg)
    {
        //接收传过来的信息
        RspStrong data = msg.rspStrong;
        //计算之前战斗力
        int zhanliPre = PECommon.GetFightByProps(GameRoot.Instance.PlayerData);
        //对客户端的玩家数据进行修改
        GameRoot.Instance.SetPlayerDataByStrong(data);
        int zhanliNew = PECommon.GetFightByProps(GameRoot.Instance.PlayerData);

        GameRoot.AddTips("战力提升:" + (zhanliNew - zhanliPre));

        //刷新UI
        strongWnd.UpdateStrongUI();
        mainCityWnd.RefreshUI();
    }

    public void OpenStrongWnd()
    {
        //auSvc.PlayUIMusic(Constants.ClickUI);
        strongWnd.SetWndState(true);
    }

    #endregion

    #region 聊天窗口

    public void OpenChatWnd()
    {
        //auSvc.PlayUIMusic(Constants.ClickUI);
        chatWnd.SetWndState(true);
    }

    //处理传入过来的广播消息
    public void PshChat(GameMsg msg)
    {
        string name = msg.pshChat.name;
        string chat = msg.pshChat.chatTxt;

        chatWnd.AddChatMsg(name, chat);
    }

    #endregion

    #region 购买窗口
    public void OpenBuyWnd(int type)
    {
        buyWnd.SelectType(type);
        buyWnd.SetWndState(true);
    }

    public void RspBuy(GameMsg msg)
    {
        RspBuy data = msg.rspBuy;

        GameRoot.Instance.SetPlayerDataByBuy(data);
        GameRoot.AddTips("购买成功");

        if(msg.pshTaskPrgs != null)
        {
            PshTaskPrgs(msg);
        }

        mainCityWnd.RefreshUI();
        buyWnd.SetWndState(false);
    }

    public void PshPower(GameMsg msg)
    {
        PshPower data = msg.pshPower;

        //处理数据，调用GameRoot对玩家数据进行修改
        GameRoot.Instance.SetPlayerDataByPower(data);
    }
    #endregion

    #region 任务与奖励

    public void OpenTaskWnd()
    {
        taskWnd.SetWndState(true);
    }

    public void RspTaskReward(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerDataByTaskReward(msg.rspTaskReward);

        mainCityWnd.RefreshUI();
        taskWnd.RefreshUI();
    }

    public void PshTaskPrgs(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerDataByTaskPrgs(msg.pshTaskPrgs);
        //taskWnd.RefreshUI();
    }

    #endregion


}