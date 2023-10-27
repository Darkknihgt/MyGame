/****************************************************
    文件：FubenWnd.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/23 21:52:44
	功能：副本窗口
*****************************************************/

using UnityEngine;
using PEProtocol;
using UnityEngine.UI;

public class FubenWnd : WindowRoot
{
    public PlayerData pd;
    public Transform pointerTrans;
    public Button[] fbBtnArr;

    protected override void InitWnd()
    {
        pd = GameRoot.Instance.PlayerData;
        base.InitWnd();

        RefreshUI();
    }

    public void RefreshUI()
    {
        //根据任务的副本ID进行显示控制
        int fid = pd.fuben;
        for(int i = 0;i < fbBtnArr.Length; i++)
        {
            if(i < (fid % 10000))
            {
                SetActive(fbBtnArr[i].gameObject);
                if(i == (fid % 10000) - 1)
                {
                    pointerTrans.SetParent(fbBtnArr[i].transform);
                    pointerTrans.localPosition = new Vector3(25, 100, 0);
                }
            }
            else
            {
                SetActive(fbBtnArr[i].gameObject, false);
            }
        }
    }

    public void ClickTaskBtn(int fbid)
    {
        auSvc.PlayUIMusic(Constants.ClickUI);

        //体力消耗判断
        int power = resSvc.GetMapInfo(fbid).power;
        if(power > pd.power)
        {
            GameRoot.AddTips("体力不足");
        }
        else
        {
            //发送网络请求
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqFBFight,
                reqFBFight = new ReqFBFight
                {
                    fbid = fbid,
                },
            };
            netSvc.SendMsg(msg);
        }
    }

    public void ClickFubenClose()
    {
        auSvc.PlayUIMusic(Constants.ClickUI);
        this.SetWndState(false);
    }
}