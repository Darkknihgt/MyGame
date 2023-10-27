/****************************************************
    文件：CreateWnd.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/14 9:30:23
	功能：角色创建界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;


public class CreateWnd : WindowRoot
{
    public InputField rdName;

    protected override void InitWnd()
    {
        base.InitWnd();
        rdName.text = resSvc.GetRDNameData(false);
    }

    public void ClickRandBtn()
    {
        auSvc.PlayUIMusic(Constants.ClickUI);

        rdName.text = resSvc.GetRDNameData(false);
    }

    public void ClickEnterBtn()
    {
        auSvc.PlayUIMusic(Constants.UIlogin);
        string _name = rdName.text;
        //TODO 进入游戏
        if(rdName.text == "")
        {
            //TODO
            GameRoot.AddTips("当前名字不符合规范");
        }
        else
        {
            GameMsg msg = new GameMsg {
                cmd = (int)CMD.ReqRename,
                reqRename = new ReqRename
                {
                    name = _name
                }
            };
            netSvc.SendMsg(msg);
        }
    }
}