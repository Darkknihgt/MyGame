/****************************************************
    文件：BattleEndWnd.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/9/19 0:42:13
	功能：战斗结算界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class BattleEndWnd : WindowRoot
{
    #region UI Define

    public Transform rewardTrans;
    public Button btnClose;
    public Button btnExit;
    public Button btnSure;

    public Text txtTime;
    public Text txtRestHP;
    public Text txtReward;
    public Animation anim;

    #endregion

    private FBEndType endType = FBEndType.None;

    protected override void InitWnd()
    {
        base.InitWnd();

        RefreshUI();
    }

    private void RefreshUI()
    {
        switch (endType)
        {
            case FBEndType.Pause:
                SetActive(rewardTrans, false);
                SetActive(btnExit.gameObject);
                SetActive(btnClose.gameObject);
                break;

            case FBEndType.Win:
                SetActive(rewardTrans, false);
                SetActive(btnExit.gameObject,false);
                SetActive(btnClose.gameObject,false);

                MapConfigs cfg = resSvc.GetMapInfo(fbid);
                int min = costTime / 60;
                int sec = costTime % 60;
                int coin = cfg.coin;
                int exp = cfg.exp;
                int crystal = cfg.crystal;
                SetText(txtTime, "通关时间： " + min + " : " + sec);
                SetText(txtRestHP, "剩余血量: " + restHP);
                SetText(txtReward, "关卡奖励: " + Constants.Color(coin + " 金币 ", TxtColor.Green) + Constants.Color(exp + " 经验 ", TxtColor.Yellow)
                    + Constants.Color(crystal + " 水晶 ", TxtColor.Blue));

                timerSvc.AddTimeTask((int tid) => {
                SetActive(rewardTrans, true);
                anim.Play();
                timerSvc.AddTimeTask((int tid1) => {
                    auSvc.PlayUIMusic(Constants.UIStrong);
                    timerSvc.AddTimeTask((int tid2) => {
                        auSvc.PlayUIMusic(Constants.UIStrong);
                        timerSvc.AddTimeTask((int tid3) => {
                            auSvc.PlayUIMusic(Constants.UIStrong);
                            timerSvc.AddTimeTask((int tid4) => {
                                auSvc.PlayUIMusic(Constants.UIStrong);
                            }, 150);
                        }, 200);
                    }, 200);
                },300);

                },1000);
                break;

            case FBEndType.Lose:
                SetActive(rewardTrans, false);
                SetActive(btnExit.gameObject);
                SetActive(btnClose.gameObject,false);
                auSvc.PlayUIMusic(Constants.FBLose);
                break;
        }
    }

    public void ClickClose()
    {
        auSvc.PlayUIMusic(Constants.ClickUI);
        BattleSys.Instance.battleMgr.isPauseGame = false;
        SetWndState(false);
    }

    public void ClickExit()
    {
        auSvc.PlayUIMusic(Constants.ClickUI);
        //进入主城，销毁当前战斗
        MainCitySys.Instance.EnterMainCity();
        BattleSys.Instance.DestroyBattle();
        
    }

    public void ClickSureBtn()
    {
        auSvc.PlayUIMusic(Constants.ClickUI);
        //进入主城，销毁当前战斗
        //打开副本
        MainCitySys.Instance.EnterMainCity();
        BattleSys.Instance.DestroyBattle();
        FubenSys.Instance.SetFubenWndState(true);

    }

    public void SetWndType(FBEndType endType)
    {
        this.endType = endType;
    }

    private int fbid;
    private int costTime;
    private int restHP;

    public void BattleEndData(int fbid,int costTime,int resthp)
    {
        this.fbid = fbid;
        this.costTime = costTime;
        this.restHP = resthp;
    }
}

public enum FBEndType
{
    None,
    Pause,
    Win,
    Lose,
}