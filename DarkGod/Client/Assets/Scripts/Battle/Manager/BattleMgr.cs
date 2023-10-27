/****************************************************
    文件：BattleMgr.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/24 22:6:30
	功能：战斗管理器
*****************************************************/

using PEProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleMgr : MonoBehaviour
{
    private StateMgr stateMgr = null;
    private SkillMgr skillMgr = null;
    private MapMgr mapMgr = null;
    private ResSvc resSvc = null;
    private AudioSvc auSvc = null;
    private MapConfigs mapData;

    public EntityPlayer entitySelfPlayer;
    //private PlayerController playerCtrl;
    private Dictionary<string, EntityMonster> monsterEntityDic = new Dictionary<string, EntityMonster>();  //用来存储生成的monsterentity



    public void Init(int mapid,Action cb = null)
    {
        resSvc = ResSvc.Instance;


        auSvc = AudioSvc.Instance;
        stateMgr = transform.gameObject.AddComponent<StateMgr>();
        stateMgr.Init();
        skillMgr = transform.gameObject.AddComponent<SkillMgr>();
        skillMgr.Init();


        mapData = resSvc.GetMapInfo(mapid);
        resSvc.AsyncLoadScene(mapData.sceneName, () =>
        {

            //初始化地图数据
            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
            mapMgr = map.GetComponent<MapMgr>();
            mapMgr.Init(this);

            //设置地图信息(规范)
            map.transform.localPosition = Vector3.zero;
            map.transform.localScale = Vector3.one;

            //设置主相机
            Camera.main.transform.position = mapData.mainCamPos;
            Camera.main.transform.localEulerAngles = mapData.mainCamRote;

            LoadPlayer(mapData);

            entitySelfPlayer.Idle();

            //延迟激活该场景中的第一批怪物
            ActiveCurrentBatchMonsters();

            auSvc.PlayBGMusic(Constants.BGHuangYe);

            if(cb != null)
            {
                cb();
            }

        });



    }

    public bool triggerCheck = true;
    public bool isPauseGame = false;
    public void Update()
    {
        foreach(var item in monsterEntityDic)
        {
            EntityMonster em = item.Value;
            em.TickAILogic();
        }

        if(mapMgr != null)
        {
            if(triggerCheck && monsterEntityDic.Count == 0)
            {
                bool isExit = mapMgr.SetNextTriggerOn();
                triggerCheck = false;

                if(isExit == false)//表面没有下一波了，开始结算
                {
                    //TODO
                    EndBattle(true,entitySelfPlayer.HP);
                }
            }
        }
    }

    public void EndBattle(bool isWin,int restHP)
    {
        //停止播放战斗音乐
        isPauseGame = true;
        AudioSvc.Instance.StopBGMusic();
        BattleSys.Instance.EndBattle(isWin, restHP);

    }

    public void LoadPlayer(MapConfigs mapData)
    {
        GameObject player = resSvc.LoadPrefabs(PathDefine.AssassinBattlePlayerPrefab);
        player.transform.position = mapData.playerBornPos;
        player.transform.localEulerAngles = mapData.playerBornRote;
        player.transform.localScale = Vector3.one;

        //对玩家属性的设置
        PlayerData pd = GameRoot.Instance.PlayerData; //获得玩家的属性
        BattleProps props = new BattleProps
        {
            hp = pd.hp,
            ad = pd.ad,
            ap = pd.ap,
            addef = pd.addef,
            apdef = pd.apdef,

            dodge = pd.dodge,
            pierce = pd.pierce,
            critical = pd.critical,
        };

        entitySelfPlayer = new EntityPlayer()
        {
            battleMgr = this,
            stateMgr = stateMgr,
            skillMgr = skillMgr,
        };
        entitySelfPlayer.Name = "AssassinBattle";
        entitySelfPlayer.SetBattleProps(props);//将获取的玩家战斗属性赋值给玩家的存储属性中

        PlayerController playerCtrl = player.GetComponent<PlayerController>(); // 获取副本中玩家身上的角色行为控制脚本
        playerCtrl.Init();
        entitySelfPlayer.SetCtrl(playerCtrl);//将该玩家的角色控制脚本注入到实体数据脚本中
        //entitySelfPlayer.controller = playerCtrl; 
       
    }

    public void LoadMonsterByWaveID(int wave)
    {
        for (int i = 0; i < mapData.monsterLst.Count; i++)
        {
            MonsterData md = mapData.monsterLst[i];
            if (md.mWave == wave) //列表中的怪物信息的波数等于传递的波数，则生成该怪物
            {
                GameObject monster = resSvc.LoadPrefabs(md.mCfg.resPath);
                monster.transform.localPosition = md.mBornPos;
                monster.transform.localEulerAngles = md.mBornRote;
                monster.transform.localScale = Vector3.one;

                monster.name = "m" + md.mWave + "_" + md.mIndex;
                EntityMonster em = new EntityMonster()  //个人感觉有点问题，怪物的状态和技能管理怎么能和人物使用一个？
                {
                    battleMgr = this,
                    stateMgr = stateMgr,
                    skillMgr = skillMgr,
                };

                em.md = md;
                em.SetBattleProps(md.mCfg.props); //为生成的怪物进行属性灌注
                em.Name = monster.name;

                MonsterController mc = monster.GetComponent<MonsterController>();
                mc.Init();
                em.controller = mc;

                monster.SetActive(false);                
                monsterEntityDic.Add(monster.name, em);

                if(md.mCfg.mType == MonsterType.Normal)
                {
                    GameRoot.Instance.dynamicWnd.AddHPItemInfo(monster.name, mc.HpRoot, em.HP);//怪物生成的时候，其在动态窗口里的血条也跟着生成
                }
                else if(md.mCfg.mType == MonsterType.Boss)
                {
                    //激活boss血条
                    BattleSys.Instance.playerWnd.SetBossHPBarState(true);
                }
            }
        }
    }

    public List<EntityMonster> GetEntityMonster()
    {
        List<EntityMonster> lst = new List<EntityMonster>();
        foreach (var item in monsterEntityDic)
        {
            lst.Add(item.Value);
        }
        return lst;
    }

    public void RemoveMonster(string key)
    {
        EntityMonster entityMonster;
        if (monsterEntityDic.TryGetValue(key, out entityMonster))
        {
            monsterEntityDic.Remove(key);
            GameRoot.Instance.dynamicWnd.RemoveItemInfo(key);
        }
    }

    public void ActiveCurrentBatchMonsters()
    {
        TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            foreach (var item in monsterEntityDic)
            {
                item.Value.controller.gameObject.SetActive(true);
                item.Value.Born();
                TimerSvc.Instance.AddTimeTask((int id) =>
                {
                    item.Value.Idle();
                }, 1000);
            }
        }, 800);
    }

    public void SetPlayerDir(Vector2 dir)
    {
        //PECommon.Log(dir.ToString());
        if (entitySelfPlayer.canControl == false)
        {
            return;
        }

        if (entitySelfPlayer.currentAniState == AniState.Idle || entitySelfPlayer.currentAniState == AniState.Move)
        {
            if(dir == Vector2.zero){
                entitySelfPlayer.Idle();               
            }
        else
            {
                entitySelfPlayer.Move();
                entitySelfPlayer.SetDir(dir);
            }
        }
    }


    public void ReqReleaseSkill(int id)
    {
        switch (id)
        {
            case 0:
                ReleaseNormalAtk();
                break;
            case 1:
                ReleaseSkill1();
                break;
            case 2:
                ReleaseSkill2();
                break;
            case 3:
                ReleaseSkill3();
                break;
        }
    }

    #region 释放技能
    private int[] comboArr = new int[] { 111, 112, 113, 114, 115 };
    public double lastAtkTime = 0;
    public int comboIndex = 0;
    private void ReleaseNormalAtk()
    {
        if (entitySelfPlayer.currentAniState == AniState.Attack) //当玩家状态处于攻击状态时
        {
            double nowAtkTime = TimerSvc.Instance.GetNowTime();
            if (nowAtkTime - lastAtkTime < Constants.AtkTimeSpace && lastAtkTime != 0)
            {
                if (comboIndex != comboArr.Length - 1)
                {
                    comboIndex += 1;
                    entitySelfPlayer.comboQue.Enqueue(comboArr[comboIndex]);
                    lastAtkTime = nowAtkTime;
                }
                else
                {
                    lastAtkTime = 0;
                    comboIndex = 0;                   
                }
            }
        }
        else if (entitySelfPlayer.currentAniState == AniState.Idle || entitySelfPlayer.currentAniState == AniState.Move)
        {
            lastAtkTime = TimerSvc.Instance.GetNowTime();
            entitySelfPlayer.Attack(comboArr[0]);

        }
    }

    private void ReleaseSkill1()
    {
        //PECommon.Log("Click Skill 1");
        entitySelfPlayer.Attack(101);
    }

    private void ReleaseSkill2()
    {
        //PECommon.Log("Click Skill 2");
        entitySelfPlayer.Attack(102);
    }

    private void ReleaseSkill3()
    {
        //PECommon.Log("Click Skill 3");
        entitySelfPlayer.Attack(103);
    }
    #endregion

    ///技能是否允许释放
    public bool CanRlsSkill()
    {
        return entitySelfPlayer.canRlsSkill;
    }
}