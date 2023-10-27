/****************************************************
    文件：ChatWnd.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/16 23:8:27
	功能：聊天窗口
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PEProtocol;
using System.Collections;

public class ChatWnd : WindowRoot
{
    #region 字段
    //获取三个按钮
    public Image imgWorld;
    public Image imgGuild;
    public Image imgFriend;

    //txt组件
    public Text chatTxt;

    public InputField inpTxt;

    //聊天方式
    private int chatType;

    private bool canSend = true;
    #endregion

    protected override void InitWnd()
    {
        base.InitWnd();
        chatType = 0; //表示当前为世界聊天

        RefreshUI();
    }


    private List<string> chatList = new List<string>(); //表，收纳聊天语句
    /// <summary>
    /// 更新聊天界面
    /// </summary>   
    public void RefreshUI()
    {
        if(chatType == 0)
        {
            string chatMsg = "";//用来屏幕表示的文字
            for(int i = 0; i < chatList.Count; i++) //表示每句话后执行一次换行
            {
                chatMsg += chatList[i] + "\n";
            }
            SetText(chatTxt, chatMsg); //将文字刷新到对话框中

            //更新图标类型
            SetSprite(imgWorld, PathDefine.ChatBgOn);
            SetSprite(imgGuild, PathDefine.ChatBgOff);
            SetSprite(imgFriend, PathDefine.ChatBgOff);
        }
        else if(chatType == 1)
        {
            SetText(chatTxt, "尚未加入工会");
            //更新图标类型
            SetSprite(imgWorld, PathDefine.ChatBgOff);
            SetSprite(imgGuild, PathDefine.ChatBgOn);
            SetSprite(imgFriend, PathDefine.ChatBgOff);
        }
        else if(chatType == 2)
        {
            SetText(chatTxt, "暂无好友信息");

            //更新图标类型
            SetSprite(imgWorld, PathDefine.ChatBgOff);
            SetSprite(imgGuild, PathDefine.ChatBgOff);
            SetSprite(imgFriend, PathDefine.ChatBgOn);
        }
    }

    //将传递过来的信息打印在屏幕上
    public void AddChatMsg(string name,string chat)
    {
        chatList.Add(Constants.Color(name + ":", TxtColor.Blue) + chat);

        if(chatList.Count > 11)
        {
            chatList.RemoveAt(0);          
        }
        if (gameObject.activeSelf) //防止联机时因为聊天窗口未打开而产生的bug
        {
            RefreshUI();
        }
  
    }

    #region 点击事件
    public void ClickCloseWnd()
    {
        auSvc.PlayUIMusic(Constants.ClickUI);
        SetWndState(false);
    }
    public void ClickWorld()  //点击世界聊天后，将聊天类型设置为0，刷新UI
    {
        auSvc.PlayUIMusic(Constants.ClickUI);

        chatType = 0;
        RefreshUI();
    }
    public void ClickGuild()
    {
        auSvc.PlayUIMusic(Constants.ClickUI);

        chatType = 1;
        RefreshUI();

    }
    public void ClickFriend()
    {
        auSvc.PlayUIMusic(Constants.ClickUI);

        chatType = 2;
        RefreshUI();

    }

    public void ClickSendMsg()//发送玩家消息给服务器
    {

        if (canSend == false)
        {
            GameRoot.AddTips("世界聊天每隔5s发送一次");
            return;
        }
        //先对消息进行判断
        if (inpTxt.text != null && inpTxt.text !="" && inpTxt.text != " ")
        {
            //二次判断，消息要限制字数
            if(inpTxt.text.Length > 20)
            {
                GameRoot.AddTips("输入信息不能超过12个字");
            }
            else
            {
                //将消息发送给服务端
                GameMsg msg = new GameMsg
                {
                    cmd = (int)CMD.SndChat,
                    sndChat = new SndChat
                    {
                        chatTxt = inpTxt.text,
                    }
                };
                //将消息框清空
                inpTxt.text = "";              
                netSvc.SendMsg(msg);
                canSend = false;

                timerSvc.AddTimeTask((int tid) => {
                    canSend = true;
                }, 5, PETimeUnit.Second);
            }
        }
        else
        {
            GameRoot.AddTips("尚未输入聊天信息");
        }
    }
    #endregion

    //使用协程进行计时
    //IEnumerator Timer()
    //{
    //    yield return new WaitForSeconds(5f);

    //    canSend = true;
    //}
}