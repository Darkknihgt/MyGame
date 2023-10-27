/****************************************************
    文件：TaskWnd.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/20 23:46:4
	功能：任务奖励界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;
using System.Collections.Generic;
using System;
using PENet;

public class TaskWnd : WindowRoot
{
    private PlayerData pd = null;
    private List<TaskRewardData> trDataLst = new List<TaskRewardData>(); //用来存储任务奖励的进度数据

    public Transform ContentTrans;

    protected override void InitWnd()
    {
        base.InitWnd();

        pd = GameRoot.Instance.PlayerData;

        RefreshUI();
    }

    public void RefreshUI()
    {
        trDataLst.Clear(); //每次打开刷新UI时，先将任务列表清空

        List<TaskRewardData> todoTask = new List<TaskRewardData>(); //未领取清单
        List<TaskRewardData> doneTask = new List<TaskRewardData>(); //领取清单

        string[] taskInfo = pd.taskArr; //获取玩家当前的任务奖励信息
        //解析玩家信息
        for(int i = 0; i < taskInfo.Length; i++)
        {
            string[] strInfo =taskInfo[i].Split('|');
            TaskRewardData trData = new TaskRewardData();
            trData.id = int.Parse(strInfo[0]);
            trData.prgs = int.Parse(strInfo[1]);
            trData.taked = strInfo[2].Equals("1");

            if (trData.taked)
            {
                doneTask.Add(trData);
            }
            else
            {
                todoTask.Add(trData);
            }
        }
        trDataLst.AddRange(todoTask);
        trDataLst.AddRange(doneTask);

        //清除UI列表里的重复item
        for(int i =0;i < ContentTrans.childCount; i++)
        {
            Destroy(ContentTrans.GetChild(i).gameObject);
        }

        //创建预制体
        for(int i = 0; i < trDataLst.Count; i++)
        {
            GameObject go = resSvc.LoadPrefabs(PathDefine.ItemTaskPrefab);
            go.transform.SetParent(ContentTrans);

            //对生成的对象进行设置
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.name = "ItemTask_" + i;

            TaskRewardData trData = trDataLst[i];
            TaskRewardCfg trCfg = resSvc.GetTaskRewardCfg(trData.id);
            //对生成的任务奖励栏进行UI设置
            SetText(GetItemTrans(go.transform, "txtName"), trCfg.taskName);
            SetText(GetItemTrans(go.transform, "txtExp"), "奖励:      经验"+trCfg.exp);
            SetText(GetItemTrans(go.transform, "txtcoin"), "金币"+trCfg.coin);
            SetText(GetItemTrans(go.transform, "txtPrg"), trData.prgs+"/" + trCfg.count);
            float prgVal = (trData.prgs * 1.0f) / trCfg.count;
            Image imgPrgVal = GetItemTrans(go.transform, "prgBar/prgVal").GetComponent<Image>();
            imgPrgVal.fillAmount = prgVal;

            //对按钮进行设置
            Button btnTaked = GetItemTrans(go.transform, "btnTake").GetComponent<Button>();
            //按钮的点击事件
            btnTaked.onClick.AddListener(() =>
            {
                ClickTakeBtn(go.name);
            });

            //判断完成任务的图标与逻辑
            Transform transComp = GetItemTrans(go.transform, "imgComp");
            if (trData.taked)
            {
                //当表示已经获取
                btnTaked.interactable = false;
                SetActive(transComp, true);
            }
            else
            {
                //未获取
                SetActive(transComp, false);
                if(trData.prgs == trCfg.count)
                {
                    btnTaked.interactable = true;
                }
                else
                {
                    btnTaked.interactable = false;
                }
            }
        }
    }

    private void ClickTakeBtn(string name)
    {
        Debug.Log("Name:" + name);

        //当前按钮按下，发送信息给客户端
        string[] taskNameLst =name.Split('_');
        int taskid = int.Parse(taskNameLst[1]);

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.ReqTaskReward,
            reqTaskReward = new ReqTaskReward
            {
                tid = trDataLst[taskid].id,
            }           
        };
        netSvc.SendMsg(msg);

        //发送完后开始进行提示
        TaskRewardCfg trc = resSvc.GetTaskRewardCfg(trDataLst[taskid].id);
        int coin = trc.coin;
        int exp = trc.exp;
        GameRoot.AddTips(Constants.Color(("获得金币奖励 +" + coin + "    获得经验奖励 +" + exp), TxtColor.Blue));
    }


    public void ClickCloseTaskWnd()
    {
        auSvc.PlayUIMusic(Constants.ClickUI);
        this.SetWndState(false);
    }
}