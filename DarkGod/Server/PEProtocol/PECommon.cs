/****************************************************
    文件：ServerSession.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/15 23:5:29
	功能：通用工具
*****************************************************/


using PENet;
using PEProtocol;

public enum LogType
{
    Log = 0,
    Warn = 1,
    Error = 2,
    Info = 3
}
public class PECommon
{
    public static void Log(string msg = "",LogType tp = LogType.Log)
    {
        LogLevel lv = (LogLevel)tp;
        PETool.LogMsg(msg, lv);
    }

    public static int GetFightByProps(PlayerData pd)
    {
        return pd.lv * 100 + pd.ad + pd.ap + pd.addef + pd.apdef;
    }

    public static int GetPowerLimit(int lv)
    {
        return (lv - 1) / 10 * 150 + 150;
    }

    public static int GetExpUpValByLv(int lv)
    {
        return 100 * lv * lv;
    }

    public static void CalcExp(PlayerData pd, int addExp)
    {
        int curtLv = pd.lv;
        int curtExp = pd.exp;
        int addRestExp = addExp; //需要添加经验

        while (true)
        {
            int upNeedExp = PECommon.GetExpUpValByLv(curtLv) - curtExp; //获取当前升级所需要的经验
            if (curtExp + addRestExp > upNeedExp)//当当前经验与添加经验的总和大于所需经验时
            {
                curtLv += 1;//升一级
                curtExp = 0; //经验条归0
                addRestExp -= upNeedExp;
            }
            else//当当前经验与添加经验之和不满足升级条件时
            {
                pd.lv = curtLv;
                pd.exp = curtExp + addRestExp;
                break;
            }
        }
    }

    public static int PowerAddSpace = 5;
    public static int PowerAddCount = 2;
}

