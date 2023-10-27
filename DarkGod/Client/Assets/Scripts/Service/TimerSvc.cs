/****************************************************
    文件：NetSvc.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/17 18:39:15
	功能：定时器服务
*****************************************************/


using System;

public class TimerSvc : SystemRoot
{
    public static TimerSvc Instance = null;
    private PETimer pt;

    public void InitTimerSvc()
    {
        Instance = this;
        pt = new PETimer();

        //设置日志输出
        pt.SetLog((string info)=> {
            PECommon.Log(info);
        });

        PECommon.Log("Init TimerSvc...");
    }

    public void Update()
    {
        pt.Update();
    }

    public int AddTimeTask(Action<int> callback,double delay,PETimeUnit timeUnit = PETimeUnit.Millisecond,int count = 1)
    {
        return pt.AddTimeTask(callback, delay, timeUnit, count);
    }

    public double GetNowTime()
    {
        return pt.GetMillisecondsTime();
    }

    public void DelTask(int tid)
    {
        pt.DeleteTimeTask(tid);
    }
}

