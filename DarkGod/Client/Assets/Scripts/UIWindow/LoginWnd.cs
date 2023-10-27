/****************************************************
    文件：LoginWnd.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/12 13:17:22
	功能：处理登陆界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class LoginWnd : WindowRoot
{
    public InputField ipAcct;
    public InputField iptPass;
    public Button btnEnter;
    public Button btnNotice;

    //初始化
    protected override void InitWnd()
    {
        base.InitWnd();
        //判断是否存储了账号密码
        if (PlayerPrefs.HasKey("Acct") && PlayerPrefs.HasKey("Pass"))
        {
            ipAcct.text = PlayerPrefs.GetString("Acct");
            iptPass.text = PlayerPrefs.GetString("Pass");
        }
        else
        {
            ipAcct.text = "";
            iptPass.text = "";
        }

        

    }
  
    //点击进入游戏按钮
    public void ClickEnterBtnLogin()
    {
        //音效引入
        auSvc.PlayUIMusic(Constants.UIlogin);

        //判断密码是否输入正确
        string _acct = ipAcct.text;
        string _pass = iptPass.text;
        if(_acct !="" && _pass != "")
        {
            PlayerPrefs.SetString("Acct", _acct);
            PlayerPrefs.SetString("Pass", _pass);

            //发送网络请求TODO
            GameMsg msg = new GameMsg() {
                cmd = (int)CMD.ReqLogin,
                reqLogin = new ReqLogin()
                {
                    acct = _acct,
                    pass = _pass
                }
            };
            netSvc.SendMsg(msg);

            //调用RepLogin函数,toremove
            //LoginSys.Instance.RspLogin(msg);
        }
        else
        {
            GameRoot.AddTips("账号或密码为空");
        }
    }

    public void ClickNoticeBtn()
    {
        auSvc.PlayUIMusic(Constants.ClickUI);

        //消息弹窗
        GameRoot.AddTips("功能正在开发中...");
    }
}