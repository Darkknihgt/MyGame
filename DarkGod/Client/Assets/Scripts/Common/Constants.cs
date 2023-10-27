/****************************************************
    文件：Constants.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/12 0:13:28
	功能：常量类
*****************************************************/

using UnityEngine;

public enum TxtColor
{
    Red,
    Green,
    Blue,
    Yellow,
}

public enum DamageType
{
    None =0,
    AD = 1,
    AP =2,
}

public enum EntityType
{
    None,
    Player,
    Monster,
}

public enum EntityState
{
    None,
    BatiState,
}

public enum MonsterType
{
    None,
    Normal = 1,
    Boss = 2,
}

public class Constants 
{
    #region 文字染色工具
    private const string ColorRed = "<color=#FF0000FF>";
    private const string ColorGreen = "<color=#00FF00FF>";
    private const string ColorBlue = "<color=#00B4FFFF>";
    private const string ColorYellow = "<color=#FFFF00FF>";
    private const string ColorEnd = "</color>";

    public static string Color(string str, TxtColor c)
    {
        string result = "";
        switch (c)
        {
            case TxtColor.Red:
                result = ColorRed + str + ColorEnd;
                break;
            case TxtColor.Green:
                result = ColorGreen + str + ColorEnd;
                break;
            case TxtColor.Blue:
                result = ColorBlue + str + ColorEnd;
                break;
            case TxtColor.Yellow:
                result = ColorYellow + str + ColorEnd;
                break;
        }
        return result;
    }

    #endregion

    //登陆背景
    public const string SceneLogin = "SceneLogin";
    public const int SceneMaincityID = 10000;
    //public const string SceneMainCity = "SceneMainCity";

    //登陆背景音乐
    public const string BGLogin = "bgLogin";
    public const string BGMainCity = "bgMainCity";
    public const string BGHuangYe = "bgHuangYe";

    //创建账号中进入游戏UI音
    public const string UIlogin = "uiLoginBtn";

    //常规UI声音
    public const string ClickUI = "uiClickBtn";
    public const string UIExtenBtn = "uiExtenBtn";
    public const string UIOpenPage = "uiOpenPage";
    public const string UIStrong = "fbitem";

    public const string FBLose = "fblose";
    public const string FBLoGoEnter = "fbwin";
    //public const string FBLose = "fblose";

    //角色音效
    public const string AssassinHit = "assassin_Hit";

    //屏幕标准高度
    public const int ScreenStandardHeight = 750;

    //屏幕标准宽度
    public const int ScreenStandardWidth = 1334;

    //屏幕摇杆操作距离
    public const int ScreenOPDos = 90;

    //玩家速度和敌人速度
    public const float playerSpeed = 8;
    public const float EnemySpeed = 3;

    //玩家动画混合转换
    public const float idleAnim = 0;
    public const float walkAnim = 1;
    public const float transSpeed = 5;

    public const float HPChangeSpeed = 0.1f;

    //Auto Task NPC ID
    public const int NPCWiseMan = 0;
    public const int NPCGeneral = 1;
    public const int NPCArtisan = 2;
    public const int NPCTrader = 3;

    //战斗参数
    public const int ActionDefault = -1;
    public const int ActionBorn = 0;
    public const int ActionDie = 100;
    public const int ActionHit = 101;

    public const int DieAniLength = 5000;

    //攻击连击时间间隔
    public const int AtkTimeSpace = 500;

}