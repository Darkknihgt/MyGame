using System;
using System.Collections.Generic;
using System.Timers;

public class PETimer
{
    private static readonly string obj = "lock";
    private static readonly string lockTime = "lock";
    private static readonly string lockFrame = "lock";

    private Action<string> taskLog;
    private Action<Action<int>, int> taskHandle;
    private DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    private double nowTime;
    private Timer srvTimer;

    private int tid;
    private List<int> tidLst = new List<int>();
    private List<int> RecTidLst = new List<int>();

    private List<PETimerTask> taskTimeLst = new List<PETimerTask>();
    private List<PETimerTask> tempTimeLst = new List<PETimerTask>();

    private int frameCounter;
    private List<PEFrameTask> taskFrameLst = new List<PEFrameTask>();
    private List<PEFrameTask> tempFrameLst = new List<PEFrameTask>();


    public PETimer(int interval = 0)
    {
        tidLst.Clear();
        RecTidLst.Clear();

        taskTimeLst.Clear();
        tempTimeLst.Clear();

        taskFrameLst.Clear();
        tempFrameLst.Clear();

        if (interval != 0)
        {
            srvTimer = new Timer(interval) { AutoReset = true };

            srvTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                Update();
            };
            srvTimer.Start();
        }
    }

    public void Update()
    {
        CheckTimeTask();
        CheckFrameTask();
        //统一回收tid
        if (RecTidLst.Count > 0)
        {
            lock (obj)
            {
                RecycleTid();
            }
        }
    }

    #region FrameTask
    public int AddFrameTask(Action<int> callback, int delay, int count = 1)
    {
        int tid = GetTid();
        lock (lockFrame)
        {
            tempFrameLst.Add(new PEFrameTask(tid, frameCounter + delay, callback, delay, count));
        }

        return tid;
    }
    public bool DeleteFrameTask(int tid)
    {
        bool exist = false;

        for (int i = 0; i < taskFrameLst.Count; i++)
        {
            PEFrameTask task = taskFrameLst[i];
            if (task.tid == tid)
            {
                taskFrameLst.RemoveAt(i);
                for (int j = 0; j < tidLst.Count; j++)
                {
                    if (tidLst[j] == tid)
                    {
                        tidLst.RemoveAt(j);
                        break;
                    }
                }
                exist = true;
                break;
            }
        }

        if (!exist)
        {
            for (int k = 0; k < tempFrameLst.Count; k++)
            {
                PEFrameTask task = tempFrameLst[k];
                if (task.tid == tid)
                {
                    tempFrameLst.RemoveAt(k);
                    for (int j = 0; j < tidLst.Count; j++)
                    {
                        if (tidLst[j] == tid)
                        {
                            tidLst.RemoveAt(j);
                            break;
                        }
                    }
                    exist = true;
                    break;
                }
            }
        }
        return exist;

    }
    public bool ReplaceFrameTask(int tid, Action<int> callback, int delay, int count = 1)
    {
        PEFrameTask newTask = new PEFrameTask(tid, frameCounter + delay, callback, delay, count);

        bool isRep = false;
        for (int i = 0; i < taskFrameLst.Count; i++)
        {
            if (tid == taskFrameLst[i].tid)
            {
                taskFrameLst[i] = newTask;
                isRep = true;
                break;
            }
        }
        if (!isRep)
        {
            for (int i = 0; i < tempFrameLst.Count; i++)
            {
                if (tid == tempFrameLst[i].tid)
                {
                    tempFrameLst[i] = newTask;
                    isRep = true;
                    break;
                }
            }
        }

        return isRep;
    }

    #endregion

    #region TimerTask
    public int AddTimeTask(Action<int> callback, float delay,
        TimeUnit timeUnit = TimeUnit.MilliSecond, int count = 1)
    {
        if (timeUnit != TimeUnit.MilliSecond)
        {
            switch (timeUnit)
            {
                case TimeUnit.Second:
                    delay = delay * 1000;
                    break;
                case TimeUnit.Minute:
                    delay = delay * 1000 * 60;
                    break;
                case TimeUnit.hour:
                    delay = delay * 1000 * 60 * 60;
                    break;
                case TimeUnit.day:
                    delay = delay * 1000 * 60 * 60 * 24;
                    break;
                default:
                    PELog("add task timeunit error ...");
                    break;
            }
        }
        int tid = GetTid();
        nowTime = GetUTCMilliseconds();
        lock (lockTime)
        {
            tempTimeLst.Add(new PETimerTask(tid, nowTime + delay, callback, delay, count));
        }


        return tid;
    }
    public bool DeleteTimeTask(int tid)
    {
        bool exist = false;

        for (int i = 0; i < taskTimeLst.Count; i++)
        {
            PETimerTask task = taskTimeLst[i];
            if (task.tid == tid)
            {
                taskTimeLst.RemoveAt(i);
                for (int j = 0; j < tidLst.Count; j++)
                {
                    if (tidLst[j] == tid)
                    {
                        tidLst.RemoveAt(j);
                        break;
                    }
                }
                exist = true;
                break;
            }
        }

        if (!exist)
        {
            for (int k = 0; k < tempTimeLst.Count; k++)
            {
                PETimerTask task = tempTimeLst[k];
                if (task.tid == tid)
                {
                    tempTimeLst.RemoveAt(k);
                    for (int j = 0; j < tidLst.Count; j++)
                    {
                        if (tidLst[j] == tid)
                        {
                            tidLst.RemoveAt(j);
                            break;
                        }
                    }
                    exist = true;
                    break;
                }
            }
        }
        return exist;

    }
    public bool ReplaceTimeTask(int tid, Action<int> callback, float delay,
        TimeUnit timeUnit = TimeUnit.MilliSecond, int count = 1)
    {
        if (timeUnit != TimeUnit.MilliSecond)
        {
            switch (timeUnit)
            {
                case TimeUnit.Second:
                    delay = delay * 1000;
                    break;
                case TimeUnit.Minute:
                    delay = delay * 1000 * 60;
                    break;
                case TimeUnit.hour:
                    delay = delay * 1000 * 60 * 60;
                    break;
                case TimeUnit.day:
                    delay = delay * 1000 * 60 * 60 * 24;
                    break;
                default:
                    PELog("add task timeunit error ...");
                    break;
            }
        }
        nowTime = GetUTCMilliseconds();

        PETimerTask newTask = new PETimerTask(tid, nowTime + delay, callback, delay, count);

        bool isRep = false;
        for (int i = 0; i < taskTimeLst.Count; i++)
        {
            if (tid == taskTimeLst[i].tid)
            {
                taskTimeLst[i] = newTask;
                isRep = true;
                break;
            }
        }
        if (!isRep)
        {
            for (int i = 0; i < tempTimeLst.Count; i++)
            {
                if (tid == tempTimeLst[i].tid)
                {
                    tempTimeLst[i] = newTask;
                    isRep = true;
                    break;
                }
            }
        }

        return isRep;
    }
    #endregion

    public void SetLog(Action<string> log)
    {
        taskLog = log;
    }
    public void SetHandle(Action<Action<int>, int> taskHandle)
    {
        this.taskHandle = taskHandle;
    }

    #region Tool
    public void CheckFrameTask()
    {
        if (tempFrameLst.Count > 0)
        {
            lock (lockFrame)
            {
                for (int tempIndex = 0; tempIndex < tempFrameLst.Count; tempIndex++)
                {
                    taskFrameLst.Add(tempFrameLst[tempIndex]);
                }
                tempFrameLst.Clear();
            }
        }
        frameCounter += 1;
        //对添加的列表进行操作
        for (int index = 0; index < taskFrameLst.Count; index++)
        {
            PEFrameTask task = taskFrameLst[index];
            if (frameCounter < task.destFrame)
            {
                continue;
            }
            else
            {
                //找到了一个时间达到要求的任务
                Action<int> cb = task.callback;
                try
                {
                    if (taskHandle != null)
                    {
                        taskHandle(cb, task.tid);
                    }
                    else
                    {
                        if (cb != null)
                        {
                            cb(task.tid);
                        }
                    }
                }
                catch (Exception e)
                {
                    PELog(e.ToString());
                }
                //任务完成后，清除列表中的该任务
                if (task.count == 1)
                {
                    taskFrameLst.RemoveAt(index);
                    index--;
                    RecTidLst.Add(task.tid);
                }
                else
                {
                    if (task.count != 0)
                    {
                        task.count--;
                    }
                    task.destFrame += task.delay;
                }

            }
        }
    }
    public void CheckTimeTask()
    {
        if (tempTimeLst.Count > 0)
        {
            lock (lockTime)
            {
                for (int tempIndex = 0; tempIndex < tempTimeLst.Count; tempIndex++)
                {
                    taskTimeLst.Add(tempTimeLst[tempIndex]);
                }
                tempTimeLst.Clear();
            }
        }

        //对添加的列表进行操作
        for (int index = 0; index < taskTimeLst.Count; index++)
        {
            PETimerTask task = taskTimeLst[index];
            if (GetUTCMilliseconds() < task.destTime)
            {
                continue;
            }
            else
            {
                //找到了一个时间达到要求的任务
                Action<int> cb = task.callback;
                try
                {
                    if (taskHandle != null)
                    {
                        taskHandle(cb, task.tid);
                    }
                    else
                    {
                        if (cb != null)
                        {
                            cb(task.tid);
                        }
                    }
                }
                catch (Exception e)
                {
                    PELog(e.ToString());
                }
                //任务完成后，清除列表中的该任务
                if (task.count == 1)
                {
                    taskTimeLst.RemoveAt(index);
                    index--;
                    RecTidLst.Add(task.tid);
                }
                else
                {
                    if (task.count != 0)
                    {
                        task.count--;
                    }
                    task.destTime += task.delay;
                }

            }
        }
    }
    public void RecycleTid()
    {
        for (int i = 0; i < RecTidLst.Count; i++)
        {
            int tid = RecTidLst[i];

            for (int j = 0; j < tidLst.Count; j++)
            {
                if (tid == tidLst[j])
                {
                    tidLst.RemoveAt(j);
                    break;
                }
            }
        }
        RecTidLst.Clear();
    }
    private int GetTid()
    {
        lock (obj)
        {
            tid += 1;

            //安全代码，以防万一
            while (true)
            {
                if (tid == int.MaxValue)
                {
                    tid = 0;
                }
                bool isUsed = false;
                for (int i = 0; i < tidLst.Count; i++)
                {
                    if (tid == tidLst[i])
                    {
                        isUsed = true;
                        break;//跳出当前循环
                    }
                }
                if (!isUsed)
                {
                    break;//说明没被使用，跳出外循环
                }
                else
                {
                    tid++;
                }
            }
            tidLst.Add(tid);
        }
        return tid;
    }
    private void PELog(string info)
    {
        if (taskLog != null)
        {
            taskLog(info);
        }
    }
    private double GetUTCMilliseconds()
    {
        TimeSpan ts = DateTime.UtcNow - startDateTime;
        return ts.TotalMilliseconds;
    }
    public void Reset()
    {
        tid = 0;
        taskLog = null;

        tidLst.Clear();
        RecTidLst.Clear();

        taskTimeLst.Clear();
        tempTimeLst.Clear();

        taskFrameLst.Clear();
        tempFrameLst.Clear();

        srvTimer.Stop();
    }
    public double GetMillisecondsTime()
    {
        return nowTime;
    }
    public DateTime GetLocalDateTime()
    {
        DateTime dt = TimeZone.CurrentTimeZone.ToLocalTime(startDateTime.AddMilliseconds(nowTime));
        return dt;
    }
    public int GetYear()
    {
        return GetLocalDateTime().Year;
    }
    public int GetMonth()
    {
        return GetLocalDateTime().Month;
    }
    public int GetDay()
    {
        return GetLocalDateTime().Day;
    }
    public int GetWeek()
    {
        return (int)GetLocalDateTime().DayOfWeek;
    }
    public string GetLocalTimeStr()
    {
        DateTime dt = GetLocalDateTime();
        string str = GetTimeStr(dt.Hour) + ":" + GetTimeStr(dt.Minute) + ":" + GetTimeStr(dt.Second);
        return str;
    }
    private string GetTimeStr(int time)
    {
        if (time < 10)
        {
            return "0" + time;
        }
        else
        {
            return time.ToString();
        }
    }
    #endregion

}

