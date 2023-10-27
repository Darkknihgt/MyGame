/****************************************************
    文件：MapMgr.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/24 22:6:30
	功能：地图管理器
*****************************************************/
using UnityEngine;

public class MapMgr : MonoBehaviour
{
    private int waveIndex = 1;//默认初始化的时候就生成第一批怪物
    private BattleMgr battleMgr = null;
    public TriggerData[] triggerArr;

    public void Init(BattleMgr battle)
    {
        battleMgr = battle;
        battleMgr.LoadMonsterByWaveID(waveIndex); 

        PECommon.Log("Init mapMgr");
    }

    public void TriggerExitMonsterBorn(TriggerData trigger,int waveIndex)
    {
        if(battleMgr != null)
        {
            BoxCollider boxCo = trigger.GetComponent<BoxCollider>();
            boxCo.isTrigger = false;

            battleMgr.LoadMonsterByWaveID(waveIndex);
            battleMgr.ActiveCurrentBatchMonsters();

            //当怪物生成后，则将battleMgr中的triggerCheck置为true，将开门的trigger打开
            battleMgr.triggerCheck = true;
        }
    }

    public bool SetNextTriggerOn()
    {
        waveIndex += 1;
        for(int i = 0;i < triggerArr.Length; i++)
        {
            if(triggerArr[i].BornIndex == waveIndex)
            {
                BoxCollider co = triggerArr[i].GetComponent<BoxCollider>();
                co.isTrigger = true;

                return true;
            }
        }
        return false;
    }
}

