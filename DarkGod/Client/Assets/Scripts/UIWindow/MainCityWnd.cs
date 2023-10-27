/****************************************************
    文件：MainCityWnd.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/19 12:55:1
	功能：主城场景窗口
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PENet;
using PEProtocol;
using UnityEngine.EventSystems;

public class MainCityWnd : WindowRoot
{
    #region UIDefine
    public Animation menuAnim;
    public Button menuBtn;

    public Text txtFight;
    public Text txtPower;
    public Image imagePowerPrg;
    public Text txtLv;
    public Text txtName;
    public Text txtExp;

    public Transform gridTrans;

    private bool menuState = true;
    private float pointDis;
    private Vector2 startPos = Vector2.zero;
    private Vector2 defaultPos ;

    //AUTO Task System
    public GuideConfigs taskData;
    public Button btnGuide;

    //摇杆参数
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;
    #endregion

    #region main functions
    protected override void InitWnd()
    {
        base.InitWnd();
        pointDis = (Screen.height * 1.0f / Constants.ScreenStandardHeight) * Constants.ScreenOPDos;
        defaultPos = imgTouch.transform.position;

        SetActive(imgDirPoint.gameObject, false);

        RefreshUI();
        RegisterTouchEvts();
    }

    public void RefreshUI()
    {
        PlayerData pd = GameRoot.Instance.PlayerData;
        //战斗力显示
        SetText(txtFight, PECommon.GetFightByProps(pd));
        //体力显示
        SetText(txtPower, "体力:"+pd.power+"/"+PECommon.GetPowerLimit(pd.lv));
        //体力进度条
        imagePowerPrg.fillAmount = pd.power * 1.0f / PECommon.GetPowerLimit(pd.lv);
        //等级
        SetText(txtLv, pd.lv);
        //名字
        SetText(txtName, pd.name);


        #region expprg
        GridLayoutGroup gridGroup = gridTrans.GetComponent<GridLayoutGroup>();
        //当前屏幕缩放比
        float globalRate = 1.0f * Constants.ScreenStandardHeight / Screen.height;
        float realWidth = Constants.ScreenStandardWidth * globalRate;
        float width = (realWidth - 180) / 10;

        //屏幕自适应
        gridGroup.cellSize = new Vector2(width, 7);

        int exp = (int)(pd.exp*1.0f / PECommon.GetExpUpValByLv(pd.lv) * 100);
        //经验
        SetText(txtExp, exp + "%");
        int expIndex = exp / 10;
        for(int i = 0; i < gridTrans.childCount; i++)
        {
            Image img = gridTrans.GetChild(i).GetComponent<Image>();
            if(i < expIndex)
            {
                img.fillAmount = 1;
            }
            else if(i == expIndex)
            {
                img.fillAmount = exp % 10 * 1.0f / 10;
            }
            else
            {
                img.fillAmount = 0;
            }
        }
        #endregion

        //设置自动任务导航图标
        taskData = resSvc.GetGuideDataByID(pd.guideid);
        if(taskData != null)
        {
            MainCitySys.Instance.guideWnd.RefreshTask(taskData);
            SetGuideBtnIcon(taskData.npcID);
        }
        else
        {
            SetGuideBtnIcon(-1);
        }

    }

    private void SetGuideBtnIcon(int npcID)
    {
        string spPath = "";
        Image img = btnGuide.GetComponent<Image>();
        switch (npcID)
        {
            case Constants.NPCWiseMan:
                spPath = PathDefine.WiseManHead;
                break;
            case Constants.NPCGeneral:
                spPath = PathDefine.GeneralHead;
                break;
            case Constants.NPCArtisan:
                spPath = PathDefine.ArtisanHead;
                break;
            case Constants.NPCTrader:
                spPath = PathDefine.TraderHead;
                break;
            case -1:
                spPath = PathDefine.TaskHead;
                break;
            default:
                spPath = PathDefine.TaskHead;
                break;
        }
        SetSprite(img, spPath);
    }

    private void Update()
    {
        //RefreshUI();
        
    }
    #endregion

    #region ClickFunctions

    public void ClickFubenWndBtn()
    {
        auSvc.PlayUIMusic(Constants.UIOpenPage);
        MainCitySys.Instance.EnterFubenSys();
    }

    public void ClickTaskWndBtn()
    {
        auSvc.PlayUIMusic(Constants.UIOpenPage);
        MainCitySys.Instance.OpenTaskWnd();
    }

    public void ClickBuyPowerBtn()
    {
        auSvc.PlayUIMusic(Constants.UIOpenPage);
        MainCitySys.Instance.OpenBuyWnd(0);     
    }

    public void ClickCoinBtn()
    {
        auSvc.PlayUIMusic(Constants.UIOpenPage);
        MainCitySys.Instance.OpenBuyWnd(1);
    }

    public void ClickChatBtn()
    {
        auSvc.PlayUIMusic(Constants.UIOpenPage);
        MainCitySys.Instance.OpenChatWnd();
    }

    public void ClickStrongWndBtn()
    {
        auSvc.PlayUIMusic(Constants.UIOpenPage); //播放UI界面打开音乐
        MainCitySys.Instance.OpenStrongWnd(); //通过MainCitySystem中的strongWnd引用来打开强化界面
    }

    /// <summary>
    /// 导航触发
    /// </summary>
    public void ClickAutoTask()
    {
        auSvc.PlayUIMusic(Constants.ClickUI);

        if(taskData != null)
        {
            MainCitySys.Instance.RunTask(taskData);
        }
        else
        {
            GameRoot.AddTips("更多任务正在开发中...");
        }
    }

    /// <summary>
    /// 菜单栏触发
    /// </summary>
    public void ClickMenuBtn()
    {
        auSvc.PlayUIMusic(Constants.UIExtenBtn);
        menuState = !menuState;

        AnimationClip clip = null;
        if (menuState)
        {
            clip = menuAnim.GetClip("OpenMenu");
        }
        else
        {
            clip = menuAnim.GetClip("CloseMenu");
        }
        menuAnim.Play(clip.name);
    }

    /// <summary>
    /// 角色信息触发
    /// </summary>
    public void ClickCharInfoBtn()
    {
        auSvc.PlayUIMusic(Constants.UIOpenPage);
        MainCitySys.Instance.OpenInfoWnd();
    }

    /// <summary>
    /// 摇杆位置初始化设置
    /// </summary>
    public void RegisterTouchEvts()
    {
       
        //给imgTouch对象添加PElgistener组件，并获取该组件     
        OnClickDown(imgTouch.gameObject, (PointerEventData evts) => {
            startPos = evts.position;
            SetActive(imgDirPoint.gameObject);
            imgDirBg.transform.position = evts.position;
        });

        OnClickUp(imgTouch.gameObject, (PointerEventData evts) => {
            imgDirBg.transform.position = defaultPos;
            SetActive(imgDirPoint.gameObject, false);
            imgDirPoint.transform.localPosition = Vector2.zero;
            //TODO方向信息传递
            MainCitySys.Instance.SetMoveDir(Vector2.zero);
        });

        OnClickDrag(imgTouch.gameObject, (PointerEventData evts) => {
            Vector2 dir = evts.position - startPos;
            float len = dir.magnitude;
            if(len > pointDis)
            {
                Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDis);
                imgDirPoint.transform.position = startPos + clampDir;
            }
            else
            {
                imgDirPoint.transform.position = evts.position;
            }

            MainCitySys.Instance.SetMoveDir(dir.normalized);
            Debug.Log(dir);
        });
        
    }

    #endregion

}