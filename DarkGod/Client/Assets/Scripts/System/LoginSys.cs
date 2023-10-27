/****************************************************
    文件：LoginSys.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/11 23:5:29
	功能：处理登陆业务
*****************************************************/

using PEProtocol;
using UnityEngine;

public class LoginSys : SystemRoot
{
    public static LoginSys Instance = null;

    public LoginWnd loginWnd;
    public CreateWnd createWnd;

    public override void InitSys()
    {
        Instance = this;

        PECommon.Log("LoginSys Init");
        base.InitSys();
   
    }

    /// <summary>
    /// 进入登陆界面
    /// </summary>
    public void EnterLogin()
    {
        //激活并初始化
        GameRoot.Instance.loadingWnd.SetWndState(true);
        //TODO
        //异步加载登陆场景//显示加载进度
        resSvc.AsyncLoadScene(Constants.SceneLogin,() => {
            //打开登陆界面并进行初始化
            loginWnd.SetWndState(true);
        });

        //调用背景音乐
       auSvc.PlayBGMusic(Constants.BGLogin,true);

        
        //加载完成后再打开注册登陆界面
        
    }

    /// <summary>
    /// 接收到网络服务器的登陆回应函数
    /// </summary>
    public void RspLogin(GameMsg msg)
    {
        GameRoot.AddTips("登陆成功");
        GameRoot.Instance.SetPlayerData(msg);

        if (msg.rspLogin.playerData.name == "")
        {
            //打开角色创建面板
            createWnd.SetWndState(true);
        }
        else
        {
            //TODO
            //直接跳转主城
            MainCitySys.Instance.EnterMainCity();
        }

        //关闭登陆界面
        loginWnd.SetWndState(false);
    }


    public void RspRename(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerName(msg.rspRename.name);

        //TODO
        //打开主城界面
        MainCitySys.Instance.EnterMainCity();

        //关闭创建界面
        createWnd.SetWndState(false);
    }
}