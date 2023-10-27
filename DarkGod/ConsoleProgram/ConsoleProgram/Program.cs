using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

public class Program
{
    private static readonly string obj = "lock";

    static void Main(string[] args)
    {
        Console.WriteLine("Test Start");
        //Test1();
        //TimerTest();
        Test2();
        Console.ReadKey();
    }

    static void Test1()
    {
        PETimer pt = new PETimer();
        pt.SetLog((string info) =>
        {
            Console.WriteLine(info);
        });

        pt.AddTimeTask((int tid) =>
        {
            Console.WriteLine("Time: " + DateTime.Now);
        }, 1000, TimeUnit.MilliSecond, 0);

        while (true)
        {
            pt.Update();
        }

    }

    static void TimerTest()
    {
        System.Timers.Timer t = new System.Timers.Timer(100);
        t.Elapsed += (object sender, ElapsedEventArgs args) =>
        {
            Console.WriteLine($"Process线程ID:{Thread.CurrentThread.ManagedThreadId}");
        };
        t.Start();
    }

    //独立线程检测处理任务
    static void Test2()
    {
        Queue<TaskPack> tpQue = new Queue<TaskPack>();
        PETimer pt = new PETimer(50);
        pt.AddTimeTask((int tid) =>
        {
            //Console.WriteLine("Time: " + DateTime.Now);
            Console.WriteLine($"Process线程ID:{Thread.CurrentThread.ManagedThreadId}");
        }, 100, TimeUnit.MilliSecond, 0);

        pt.SetHandle((Action<int> cb, int tid) =>
        {
            lock (obj)
            {
                if (cb != null)
                {
                    tpQue.Enqueue(new TaskPack(cb, tid));
                }

            }
        });

        while (true)
        {
            if (tpQue.Count > 0)
            {
                TaskPack tp;
                lock (obj)
                {
                    tp = tpQue.Dequeue();
                }
                tp.cb(tp.tid);
            }
        }
    }


}

public class TaskPack
{
    public Action<int> cb;
    public int tid;

    public TaskPack(Action<int> cb, int tid)
    {
        this.cb = cb;
        this.tid = tid;
    }
}

