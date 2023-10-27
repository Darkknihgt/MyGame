/****************************************************
    文件：BuyWnd.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/19 16:40:23
	功能：购买界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;

public class BuyWnd : WindowRoot
{
    public Text txtInfo;
    public Button btnSure;

    private int buyType; //0-体力 1-金币

    protected override void InitWnd()
    {
        base.InitWnd();

        RefreshUI();
        btnSure.interactable = true;
    }

    public void SelectType(int type)
    {
        this.buyType = type;
    }

    private void RefreshUI()
    {
        switch (buyType)
        {
            case 0:
                SetText(txtInfo, "是否花费" + Constants.Color("10钻石", TxtColor.Red) + "购买" + Constants.Color("100体力", TxtColor.Green) + "?"); 
                break;

            case 1:
                SetText(txtInfo, "是否花费" + Constants.Color("10钻石", TxtColor.Red) + "购买" + Constants.Color("1000金币", TxtColor.Green) + "?");
                break;
        }
    }

    public void ClickSureBtn()
    {
        auSvc.PlayUIMusic(Constants.ClickUI);
        //发送网络消息
        GameMsg msg = new GameMsg()
        {
            cmd = (int)CMD.ReqBuy,
            reqBuy = new ReqBuy
            {
                type = buyType,
                cost = 10,
            }
        };
        netSvc.SendMsg(msg);

        btnSure.interactable = false;
    }

    public void ClickCloseWnd()
    {
        auSvc.PlayUIMusic(Constants.ClickUI);
        SetWndState(false);
    }
}