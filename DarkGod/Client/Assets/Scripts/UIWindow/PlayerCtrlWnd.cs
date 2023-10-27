/****************************************************
    文件：PlayerCtrlWnd.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/25 17:15:19
	功能：战斗UI窗口
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCtrlWnd : WindowRoot
{
    #region UIDefine
    public Text txtLv;
    public Text txtName;
    public Text txtExp;
    public Transform gridTrans;  //管理经验条

    //摇杆参数
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;
    private Vector2 startPos = Vector2.zero;
    private Vector2 defaultPos;
    private float pointDis;
    private Vector2 currentDir; //处理释放技能后，不松按钮导致不移动的问题

    //玩家显示
    public Text txtSlefHP;
    public Image imgSelfHP;

    private int HPSum;

    #endregion



    protected override void InitWnd()
    {
        base.InitWnd();

        pointDis = (Screen.height * 1.0f / Constants.ScreenStandardHeight) * Constants.ScreenOPDos;
        defaultPos = imgTouch.transform.position;
        SetActive(imgDirPoint.gameObject, false);

        HPSum = GameRoot.Instance.PlayerData.hp;
        SetText(txtSlefHP, HPSum + "/" + HPSum);
        imgSelfHP.fillAmount = 1;

        SetActive(BossHPBar, false);

        SK1CDTime = resSvc.GetSkillInfo(101).cdTime/1000;
        SK2CDTime = resSvc.GetSkillInfo(102).cdTime/1000;
        SK3CDTime = resSvc.GetSkillInfo(103).cdTime/1000;

        RefreshUI();
        RegisterTouchEvts();
    }

    public void RefreshUI()
    {
        PlayerData pd = GameRoot.Instance.PlayerData;

        SetText(txtLv, pd.lv);
        SetText(txtName, pd.name);

        #region ExpPrgs
        GridLayoutGroup gridGroup = gridTrans.GetComponent<GridLayoutGroup>();
        //获得当前屏幕的缩放比
        float globalRate = 1.0f * Constants.ScreenStandardHeight / Screen.height;
        float realWidth = Constants.ScreenStandardWidth / globalRate;
        float width = (realWidth - 180) / 10;

        //屏幕自适应
        gridGroup.cellSize = new Vector2(width, 7);
        int exp = (int)(pd.exp * 1.0f / PECommon.GetExpUpValByLv(pd.lv) * 100);

        //经验
        SetText(txtExp, exp + "%");
        int expIndex = exp / 10;
        for (int i = 0; i < gridTrans.childCount; i++)
        {
            Image img = gridTrans.GetChild(i).GetComponent<Image>();
            if (i < expIndex)
            {
                img.fillAmount = 1;
            }
            else if (i == expIndex)
            {
                img.fillAmount = exp % 10 * 1.0f / 10;
            }
            else
            {
                img.fillAmount = 0;
            }
        }
        #endregion
    }

    /// <summary>
    /// 摇杆事件
    /// </summary>
    public void RegisterTouchEvts()
    {

        //给imgTouch对象添加PElgistener组件，并获取该组件     
        OnClickDown(imgTouch.gameObject, (PointerEventData evts) =>
        {
            startPos = evts.position;
            SetActive(imgDirPoint.gameObject);
            imgDirBg.transform.position = evts.position;
        });

        OnClickUp(imgTouch.gameObject, (PointerEventData evts) =>
        {
            imgDirBg.transform.position = defaultPos;
            SetActive(imgDirPoint.gameObject, false);
            imgDirPoint.transform.localPosition = Vector2.zero;
            //TODO方向信息传递
            currentDir = Vector2.zero;
            SetPlayerDir(currentDir);
        });

        OnClickDrag(imgTouch.gameObject, (PointerEventData evts) =>
        {
            Vector2 dir = evts.position - startPos;
            float len = dir.magnitude;
            if (len > pointDis)
            {
                Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDis);
                imgDirPoint.transform.position = startPos + clampDir;
            }
            else
            {
                imgDirPoint.transform.position = evts.position;
            }
            currentDir = dir.normalized;

            SetPlayerDir(currentDir);
            //Debug.Log(dir);
        });

    }

    /// <summary>
    /// 测试用
    /// </summary>
    private void Update()
    {
        //test
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ClickSkill1();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ClickSkill2();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ClickSkill3();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            ClickNormalAtk();
        }

        if (isSK1CD == true)
        {
            float delta = Time.deltaTime;
            SK1FillCount += delta;
            if (SK1FillCount >= SK1CDTime)
            {
                isSK1CD = false;
                SetActive(imgCDSK1.gameObject, false);
                SK1FillCount = 0;
            }
            else
            {
                imgCDSK1.fillAmount = 1 - SK1FillCount / SK1CDTime;
            }

            SK1txtNumCount += delta;
            if(SK1txtNumCount >= 1)
            {
                SK1txtNumCount -= 1;
                SK1Num -= 1;
                SetText(txtCDSK1, SK1Num);
            }
        }

        if (isSK2CD == true)
        {
            float delta = Time.deltaTime;
            SK2FillCount += delta;
            if (SK2FillCount >= SK2CDTime)
            {
                isSK2CD = false;
                SetActive(imgCDSK2.gameObject, false);
                SK2FillCount = 0;
            }
            else
            {
                imgCDSK2.fillAmount = 1 - SK2FillCount / SK2CDTime;
            }

            SK2txtNumCount += delta;
            if (SK2txtNumCount >= 1)
            {
                SK2txtNumCount -= 1;
                SK2Num -= 1;
                SetText(txtCDSK2, SK2Num);
            }
        }

        if (isSK3CD == true)
        {
            float delta = Time.deltaTime;
            SK3FillCount += delta;
            if (SK3FillCount >= SK3CDTime)
            {
                isSK3CD = false;
                SetActive(imgCDSK3.gameObject, false);
                SK3FillCount = 0;
            }
            else
            {
                imgCDSK3.fillAmount = 1 - SK3FillCount / SK3CDTime;
            }

            SK3txtNumCount += delta;
            if (SK3txtNumCount >= 1)
            {
                SK3txtNumCount -= 1;
                SK3Num -= 1;
                SetText(txtCDSK3, SK3Num);
            }
        }

        //boss血条变化
        if (BossHPBar.gameObject.activeSelf)
        {
            BlendBosssHP();
            imgYellow.fillAmount = currentPrg;
        }
    }


    public void SetPlayerDir(Vector2 dir)
    {
        BattleSys.Instance.battleMgr.SetPlayerDir(dir);
    }

    #region click event
    public void ClickNormalAtk()
    {
        BattleSys.Instance.battleMgr.ReqReleaseSkill(0);
    }
    public void ClickSkill1()
    {
        if (isSK1CD == false && CanRlsSkill())
        {
            BattleSys.Instance.battleMgr.ReqReleaseSkill(1);
            isSK1CD = true;
            SetActive(imgCDSK1.gameObject, true);
            imgCDSK1.fillAmount = 1;
            SK1Num = (int)SK1CDTime; //将其最大值赋给变化数
            SetText(txtCDSK1, SK1Num);
        }
    }
    public void ClickSkill2()
    {
        if (isSK2CD == false && CanRlsSkill())
        {
            BattleSys.Instance.battleMgr.ReqReleaseSkill(2);
            isSK2CD = true;
            SetActive(imgCDSK2.gameObject, true);
            imgCDSK2.fillAmount = 1;
            SK2Num = (int)SK2CDTime; //将其最大值赋给变化数
            SetText(txtCDSK2, SK2Num);
        }
        
    }
    public void ClickSkill3()
    {
        if (isSK3CD == false && CanRlsSkill())
        {
            BattleSys.Instance.battleMgr.ReqReleaseSkill(3);
            isSK3CD = true;
            SetActive(imgCDSK3.gameObject, true);
            imgCDSK3.fillAmount = 1;
            SK3Num = (int)SK3CDTime; //将其最大值赋给变化数
            SetText(txtCDSK3, SK3Num);
        }
        
    }

    public void ClickResetInit()
    {
        resSvc.ResetInit();
    }

    public void ClickHeadBtn()
    {
        BattleSys.Instance.battleMgr.isPauseGame = true;
        BattleSys.Instance.SetBattleEndWnd(FBEndType.Pause, true);
    }

    #endregion

    /// <summary>
    /// 传递到BattleMgr中
    /// </summary>
    /// <returns></returns>
    public Vector2 GetInputDir()
    {
        return currentDir;
    }

    /// <summary>
    /// 玩家血量函数
    /// </summary>
    public void SetSelfHPBarBal(int val)
    {
        SetText(txtSlefHP, val + "/" + HPSum);
        imgSelfHP.fillAmount = val * 1.0f / HPSum;
    }

    #region 技能的CD设置
    public Image imgCDSK1;
    public Text txtCDSK1;
    private bool isSK1CD = false;
    private float SK1CDTime; //这个为固定上限时间
    private int SK1Num; //变化时间

    private float SK1FillCount = 0; //图片时间判断
    private float SK1txtNumCount = 0; //文字时间判断

    /// <summary>
    /// 技能2
    /// </summary>
    public Image imgCDSK2;
    public Text txtCDSK2;
    private bool isSK2CD = false;
    private float SK2CDTime; //这个为固定上限时间
    private int SK2Num; //变化时间

    private float SK2FillCount = 0; //图片时间判断
    private float SK2txtNumCount = 0; //文字时间判断

    /// <summary>
    /// 技能3
    /// </summary>
    public Image imgCDSK3;
    public Text txtCDSK3;
    private bool isSK3CD = false;
    private float SK3CDTime; //这个为固定上限时间
    private int SK3Num; //变化时间

    private float SK3FillCount = 0; //图片时间判断
    private float SK3txtNumCount = 0; //文字时间判断

    //是否允许技能释放
    public bool CanRlsSkill()
    {
        return BattleSys.Instance.battleMgr.CanRlsSkill();
    }

    #endregion

    #region Boss血条
    public Transform BossHPBar;
    public Image imgYellow;
    public Image imgRed;
    private float currentPrg = 1f;
    private float targetPrg = 1f;

    public void SetBossHPBarState(bool state,float prg = 1)
    {
        SetActive(BossHPBar, state);
        imgYellow.fillAmount = prg;
        imgRed.fillAmount = prg;
    }

    public void SetBossHPBarVal(int oldHP,int newHP,int sumVal)
    {
        currentPrg = oldHP * 1.0f / sumVal;
        targetPrg = newHP * 1.0f / sumVal;
        imgRed.fillAmount = targetPrg;
    }

    public void BlendBosssHP()
    {
        if (Mathf.Abs(currentPrg - targetPrg) < Constants.HPChangeSpeed * Time.deltaTime)
        {
            currentPrg = targetPrg;
        }else if(currentPrg - targetPrg > 0)
        {
            currentPrg -= Constants.HPChangeSpeed * Time.deltaTime;
        }
        else
        {
            currentPrg += Constants.HPChangeSpeed * Time.deltaTime;
        }
    }
    #endregion
}