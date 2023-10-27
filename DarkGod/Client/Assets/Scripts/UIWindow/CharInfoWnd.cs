/****************************************************
    文件：CharInfoWnd.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/22 14:6:17
	功能：角色窗口显示
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharInfoWnd : WindowRoot
{
    #region 角色信息
    public PlayerData playerData;

    public Text txtCharInfo;
    public Text txtExp;
    public Image imgExpPrg;
    public Text txtPower;
    public Image imgPowerPrg;

    public Text txtJob;
    public Text txtFight;
    public Text txtHp;
    public Text txtHurt;
    public Text txtDef;

    public Button btnClose;

    //detail btn 
    public Button btnDetail;
    public Button btnDetailClose;
    public Transform detail;

    public Text dtxhp;
    public Text dtxad;
    public Text dtxap;
    public Text dtxaddef;
    public Text dtxapdef;
    public Text dtxdodge;
    public Text dtxpierce;
    public Text dtxcritical;
    #endregion

    //角色旋转
    public RawImage charShow;
    public Vector2 startPos;
    public float rotateAngle;


    protected override void InitWnd()
    {
        base.InitWnd();
        playerData = GameRoot.Instance.PlayerData;
        SetActive(detail, false);
        RefreshUI();
        RegTouchEvts();
    }

    private void RegTouchEvts()
    {
        OnClickDown(charShow.gameObject, (PointerEventData evt) => {
            startPos = evt.position;
            MainCitySys.Instance.SetStartPos();
        });

        OnClickDrag(charShow.gameObject, (PointerEventData evt) =>
         {
             rotateAngle = -(evt.position.x - startPos.x) * 0.5f;
             MainCitySys.Instance.SetPlayerRotate(rotateAngle);
         });
    }

    private void RefreshUI()
    {
        

        SetText(txtCharInfo, playerData.name + "LV." + playerData.lv);
        SetText(txtExp, playerData.exp + "/" + PECommon.GetExpUpValByLv(playerData.lv));
        imgExpPrg.fillAmount = playerData.exp*1.0f / PECommon.GetExpUpValByLv(playerData.lv);
        SetText(txtPower, playerData.power + "/" + PECommon.GetPowerLimit(playerData.lv));
        imgPowerPrg.fillAmount = playerData.power * 1.0f / PECommon.GetPowerLimit(playerData.lv);

        SetText(txtJob, " 职业   暗夜刺客");
        SetText(txtFight, " 战力   " + PECommon.GetFightByProps(playerData));
        SetText(txtHp, " 血量   " + playerData.hp);
        SetText(txtHurt, " 伤害   " + (playerData.ad + playerData.ap));
        SetText(txtDef, " 防御   " + (playerData.addef +playerData.apdef));

        //demail menu
        SetText(dtxhp, playerData.hp);
        SetText(dtxad, playerData.ad);
        SetText(dtxap, playerData.ap);
        SetText(dtxaddef, playerData.addef);
        SetText(dtxapdef, playerData.apdef);
        SetText(dtxdodge, playerData.dodge +"%");
        SetText(dtxpierce, playerData.pierce + "%");
        SetText(dtxcritical, playerData.critical + "%");

    }

    public void ClickCloseBtn()
    {
        auSvc.PlayUIMusic(Constants.ClickUI);
        MainCitySys.Instance.CloseInfoWnd();
    }

    public void ClickCloseDetailBtn()
    {
        auSvc.PlayUIMusic(Constants.ClickUI);
        SetActive(detail.gameObject, false);
    }

    public void ClickDetailBtn()
    {
        auSvc.PlayUIMusic(Constants.ClickUI);
        SetActive(detail.gameObject, true);
    }
}