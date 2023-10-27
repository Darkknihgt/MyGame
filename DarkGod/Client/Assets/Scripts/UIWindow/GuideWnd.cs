/****************************************************
    文件：GuideWnd.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/25 23:59:31
	功能：引导系统的对话界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class GuideWnd : WindowRoot
{
    public Text txtName;
    public Text txtTalk;
    public Image imgIcon;
    //public Button btnClickNext;

    private string[] dialogArr;
    private GuideConfigs curtTaskData;
    private PlayerData pd;
    private int index;

    protected override void InitWnd()
    {
        base.InitWnd();

        pd = GameRoot.Instance.PlayerData;
        index = 1;

        SetTalk();
    }

    public void RefreshTask(GuideConfigs guideCfg)
    {
        curtTaskData = guideCfg;
        dialogArr = guideCfg.dilogArr.Split('#');
    }

    private void SetTalk()
    {
        string[] talkArr = dialogArr[index].Split('|');
        if(talkArr[0] == "0")
        {
            SetSprite(imgIcon, PathDefine.SelfIcon);
        }
        else
        {
            switch (curtTaskData.npcID)
            {
                case 0:
                    SetSprite(imgIcon, PathDefine.WiseManIcon);
                    SetText(txtName, "智者");
                    break;
                case 1:
                    SetSprite(imgIcon, PathDefine.GeneralIcon);
                    SetText(txtName, "将军");
                    break;
                case 2:
                    SetSprite(imgIcon, PathDefine.ArtisanIcon);
                    SetText(txtName, "工匠");
                    break;
                case 3:
                    SetSprite(imgIcon, PathDefine.TraderIcon);
                    SetText(txtName, "商人");
                    break;
                default:
                    SetSprite(imgIcon, PathDefine.GuideIcon);
                    SetText(txtName, "小芸");
                    break;
            }
        }
        imgIcon.SetNativeSize();
        SetText(txtTalk, talkArr[1].Replace("$name", pd.name));
    }

    public void ClickNextBtn()
    {
        index += 1;
        if(index >= dialogArr.Length)
        {
            //TODO 发送任务引导完成信息
            GameMsg msg = new GameMsg()
            {
                cmd = (int)CMD.ReqGuide,
                reqGuide = new ReqGuide
                {
                    guideid = curtTaskData.id,
                }
            };
            netSvc.SendMsg(msg);

            SetWndState(false);
        }
        else
        {
            SetTalk();
        }
    }
}