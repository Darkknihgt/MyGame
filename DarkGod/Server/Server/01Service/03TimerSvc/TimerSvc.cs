/****************************************************
    文件：NetSvc.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/19 23:5:29
	功能：计时器服务
*****************************************************/
using PENet;
using PEProtocol;
using System;
using System.Collections.Generic;

public class TimerSvc
{
    private static TimerSvc instance = null;

    private PETimer pt;

    /// <summary>
    /// singleton
    /// </summary>
    public static TimerSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TimerSvc();
            }
            return instance;
        }
    }

    public void InitTimerSvc()
    {
        pt = new PETimer();

        //设置日志输出
        pt.SetLog((string info) => {
            PECommon.Log(info);
        });
        PECommon.Log("NetSvc Init Done.");
    }

    public void Update()
    {
        pt.Update();
    }

    public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUnit = PETimeUnit.Millisecond, int count = 1)
    {
        return pt.AddTimeTask(callback, delay, timeUnit, count);
    }

    public long GetNowTime()
    {
        return (long)pt.GetMillisecondsTime();
    }
}
