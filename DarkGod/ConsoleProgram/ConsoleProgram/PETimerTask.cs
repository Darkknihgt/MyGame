using System.Collections;
using System.Collections.Generic;
using System;

public class PETimerTask 
{
    public int tid;
    public double destTime;
    public Action<int> callback;
    public double delay;
    public int count;

    public PETimerTask(int tid,double destTime,Action<int> callback,double delay,int count)
    {
        this.tid = tid;
        this.destTime = destTime;
        this.callback = callback;
        this.delay = delay;
        this.count = count;
    }

}

public class PEFrameTask
{
    public int tid;
    public int destFrame;
    public Action<int> callback;
    public int delay;
    public int count;

    public PEFrameTask(int tid, int destFrame, Action<int> callback, int delay, int count)
    {
        this.tid = tid;
        this.destFrame = destFrame;
        this.callback = callback;
        this.delay = delay;
        this.count = count;
    }

}

public enum TimeUnit
{
    MilliSecond,
    Second,
    Minute,
    hour,
    day
}
