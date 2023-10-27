/****************************************************
    文件：ResSvc.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/11 23:4:56
	功能：处理资源加载
*****************************************************/

using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResSvc : MonoBehaviour
{
    public static ResSvc Instance = null;
    public void InitSvc()
    {
        PECommon.Log("ResSvc init...");
        Instance = this;
        InitCfgs();
        InitMonster();
        InitMap();
        InitGuide();
        InitStrong();
        InitTask();
        InitSkillAction();
        InitSkillMove();
        InitSkill();
    }

    public void ResetInit()
    {
        skillDic.Clear();
        skillMoveDic.Clear();
        InitSkill();
        InitSkillMove();
        PECommon.Log("Reset Done!!!");
    }

    Action prgCB = null;
    /// <summary>
    /// 异步场景加载
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="loaded"></param>
    public void AsyncLoadScene(string sceneName, Action loaded)
    {
        //加载场景
        var sceneAsync = SceneManager.LoadSceneAsync(sceneName);

        prgCB = () =>
        {
            //把加载进度传入到加载界面处理的脚本中
            GameRoot.Instance.loadingWnd.SetProgress(sceneAsync.progress);

            //加载完成后处理
            if (sceneAsync.progress == 1)
            {
                //对传进来的委托进行处理
                if (loaded != null)
                {
                    loaded();
                }

                prgCB = null;
                sceneAsync = null;
                GameRoot.Instance.loadingWnd.gameObject.SetActive(false);
            }
        };
    }

    private void Update()
    {
        if (prgCB != null)
        {
            prgCB();
        }
    }

    /// <summary>
    /// 加载声音
    /// </summary>
    Dictionary<string, AudioClip> dic = new Dictionary<string, AudioClip>();
    public AudioClip LoadAudio(string path, bool cache = false)
    {
        AudioClip au = null;
        PECommon.Log(path);
        //如果字典没有这个音乐，则读取资源，有则直接调用。
        if (!dic.TryGetValue(path, out au))
        {
            au = Resources.Load<AudioClip>(path);
            //是否进行缓存，就是存入到字典中
            if (cache)
            {
                dic.Add(path, au);
            }
        }
        return au;
    }

    Dictionary<string, GameObject> goDic = new Dictionary<string, GameObject>();
    public GameObject LoadPrefabs(string path, bool cache = false)
    {
        GameObject prefab = null;
        if (!goDic.TryGetValue(path, out prefab))
        {
            prefab = Resources.Load<GameObject>(path);
            if (cache)
            {
                goDic.Add(path, prefab);
            }
        }

        GameObject go = null;
        if (prefab != null)
        {
            go = Instantiate(prefab);
        }
        return go;
    }


    #region InitCfgs随即名字配置

    //存储表
    List<string> surnameLst = new List<string>();
    List<string> manLst = new List<string>();
    List<string> womanLst = new List<string>();

    public void InitCfgs()
    {
        //使用Unity资源系统加载文件
        TextAsset xml = Resources.Load<TextAsset>(PathDefine.RDNmaeCfg);
        if (xml == null)
        {
            PECommon.Log("xml file:" + PathDefine.RDNmaeCfg + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            //解析xml文件
            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;
            //将某一个节点转化为XmlElement
            foreach (XmlElement element in nodLst)
            {
                //如果ID为空，则跳过本次循环
                if (element.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                //int ID = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);
                foreach (XmlElement e in element.ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "surname":
                            surnameLst.Add(e.InnerText);
                            break;

                        case "man":
                            manLst.Add(e.InnerText);
                            break;

                        case "woman":
                            womanLst.Add(e.InnerText);
                            break;
                    }
                }
            }

        }
    }

    public string GetRDNameData(bool man = true)
    {
        //System.Random rd = new System.Random();

        string rdName = surnameLst[PETools.RDInt(0, surnameLst.Count - 1)];
        if (man)
        {
            rdName += manLst[PETools.RDInt(0, manLst.Count - 1)];
        }
        else
        {
            rdName += womanLst[PETools.RDInt(0, womanLst.Count - 1)];
        }

        return rdName;
    }

    #endregion

    #region taskCfg 任务奖励的数据配置

    Dictionary<int, TaskRewardCfg> taskRewardDic = new Dictionary<int, TaskRewardCfg>();
    public void InitTask()
    {
        TextAsset xmlDoc = Resources.Load<TextAsset>(PathDefine.taskCfgs);
        if (xmlDoc == null)
        {
            PECommon.Log("xml file:" + PathDefine.taskCfgs + "not exist", LogType.Error); //当没有加载到资源时，报告错误
        }
        else
        {
            XmlDocument doc = new XmlDocument();  //创建 xmlDocument类型 doc
            doc.LoadXml(xmlDoc.text);   //将xml中的文本转化为xmlDocument格式

            XmlNodeList lst = doc.SelectSingleNode("root").ChildNodes; //通过root单一节点来获取该节点中的文本列表

            foreach (XmlElement element in lst)
            {
                if (element.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                else
                {
                    int id = Convert.ToInt32(element.GetAttributeNode("ID").InnerText); //获取该子节点列表的ID的内容
                    TaskRewardCfg trCfg = new TaskRewardCfg() { id = id };

                    foreach (XmlElement e in element)
                    {
                        switch (e.Name)
                        {
                            case "taskName":
                                trCfg.taskName = e.InnerText;
                                break;
                            case "count":
                                trCfg.count = int.Parse(e.InnerText);
                                break;
                            case "coin":
                                trCfg.coin = int.Parse(e.InnerText);
                                break;
                            case "exp":
                                trCfg.exp = int.Parse(e.InnerText);
                                break;
                        }
                    }
                    taskRewardDic.Add(id, trCfg);
                }
            }
        }
    }

    public TaskRewardCfg GetTaskRewardCfg(int id)
    {
        TaskRewardCfg trCfg = null;
        if (taskRewardDic.TryGetValue(id, out trCfg))
        {
            return trCfg;
        }
        return null;
    }


    #endregion

    #region guide cfgs 自动导航任务系统配置

    Dictionary<int, GuideConfigs> guideDic = new Dictionary<int, GuideConfigs>();
    public void InitGuide()
    {
        TextAsset xml = Resources.Load<TextAsset>(PathDefine.guideCfgs);
        if (xml == null)
        {
            PECommon.Log("xml file:" + PathDefine.guideCfgs + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            //解析xml文件
            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;
            foreach (XmlElement element in nodLst)
            {
                if (element.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                else
                {
                    int id = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);
                    GuideConfigs mc = new GuideConfigs() { id = id };
                    foreach (XmlElement e in element.ChildNodes)
                    {
                        switch (e.Name)
                        {
                            case "npcID":
                                {
                                    mc.npcID = int.Parse(e.InnerText);
                                }
                                break;

                            case "dilogArr":
                                {
                                    mc.dilogArr = e.InnerText;
                                }
                                break;

                            case "actID":
                                {
                                    mc.actID = int.Parse(e.InnerText);
                                }
                                break;

                            case "coin":
                                {
                                    mc.coin = int.Parse(e.InnerText);
                                }
                                break;

                            case "exp":
                                {
                                    mc.exp = int.Parse(e.InnerText);
                                }
                                break;
                        }
                    }
                    guideDic.Add(id, mc);
                }
            }
        }
    }
    /// <summary>
    /// 外部调用来根据ID获取自动导航信息配置
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public GuideConfigs GetGuideDataByID(int id)
    {
        GuideConfigs content = null;
        if (guideDic.TryGetValue(id, out content))
        {
            return content;
        }
        return null;
    }

    /// <summary>
    /// 根据路径加载图片，并储存
    /// </summary>
    Dictionary<string, Sprite> spDic = new Dictionary<string, Sprite>();
    public Sprite LoadSprite(string path, bool cache = false)
    {
        Sprite sp = null;
        if (!spDic.TryGetValue(path, out sp))
        {
            sp = Resources.Load<Sprite>(path);
            if (cache)
            {
                spDic.Add(path, sp);
            }
        }
        return sp;
    }
    #endregion

    #region strong cfgs 强化升级配置
    //字典用来存储读取的数据配置

    private Dictionary<int, Dictionary<int, StrongConfigs>> strongDic = new Dictionary<int, Dictionary<int, StrongConfigs>>();
    public void InitStrong()
    {
        //加载文件资源
        TextAsset xml = Resources.Load<TextAsset>(PathDefine.strongCfgs);
        //解析文件
        if (xml == null)
        {
            //当xml没有读取到时，输出日志xml文件地址不存在，并返回错误提示
            PECommon.Log("xml file:" + PathDefine.guideCfgs + "not exist", LogType.Error);
        }
        else
        {
            //存在，使用xmlDocument进行加载
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml.text);

            //从root处开始解析,并且以xmlnodelist的形式获取里面所有的子节点
            XmlNodeList xmlLst = xmlDoc.SelectSingleNode("root").ChildNodes;

            //对列表中的所有节点进行遍历
            foreach (XmlElement element in xmlLst)
            {
                if (element.GetAttributeNode("ID") == null)
                {  //如果没找到某节点的ID，直接跳过
                    continue;
                }
                else
                {
                    //存储该id号，并且用类来进行存储
                    int id = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);
                    StrongConfigs sd = new StrongConfigs() { id = id };
                    //对该节点的子节点进行遍历
                    foreach (XmlElement e in element.ChildNodes)
                    {
                        int curVal = int.Parse(e.InnerText); //将子节点的字符串转化为int
                        switch (e.Name)
                        {
                            case "pos":
                                sd.pos = curVal;
                                break;
                            case "starlv":
                                sd.starlv = curVal;
                                break;
                            case "addhp":
                                sd.addhp = curVal;
                                break;
                            case "addhurt":
                                sd.addhurt = curVal;
                                break;
                            case "adddef":
                                sd.adddef = curVal;
                                break;
                            case "minlv":
                                sd.minlv = curVal;
                                break;
                            case "coin":
                                sd.coin = curVal;
                                break;
                            case "crystal":
                                sd.crystal = curVal;
                                break;
                        }
                    }
                    Dictionary<int, StrongConfigs> curDic = null;
                    if (strongDic.TryGetValue(sd.pos, out curDic))
                    {
                        curDic.Add(sd.starlv, sd);
                    }
                    else
                    {
                        curDic = new Dictionary<int, StrongConfigs>();
                        curDic.Add(sd.starlv, sd);
                        strongDic.Add(sd.pos, curDic);
                    }

                }
            }
        }

    }

    public StrongConfigs GetStrongConfigsByPosAndStarlv(int pos, int starlv)
    {
        StrongConfigs sc = null;
        Dictionary<int, StrongConfigs> scDic = null;
        if (strongDic.TryGetValue(pos, out scDic))
        {
            if (scDic.TryGetValue(starlv, out sc))
            {
                return sc;
            }
            return null;
        }

        return null;
    }

    public int GetStrongPropAddPreVal(int pos, int starlv, int type)
    {
        StrongConfigs sc = null;
        Dictionary<int, StrongConfigs> scDic = null;
        int val = 0;
        if (strongDic.TryGetValue(pos, out scDic))  //根据pos来获取对应的强化字典
        {
            for (int i = 0; i < starlv; i++)        //根据starlv获取对应的强化数据 ,从0加到当前星级
            {
                if (scDic.TryGetValue(starlv, out sc))
                {
                    switch (type)
                    {
                        case 1:
                            val += sc.addhp;
                            break;
                        case 2:
                            val += sc.addhurt;
                            break;
                        case 3:
                            val += sc.adddef;
                            break;
                    }

                }
            }

        }
        return val;
    }

    #endregion



    #region mapconfigs


    Dictionary<int, MapConfigs> mapDic = new Dictionary<int, MapConfigs>();
    public void InitMap()
    {
        TextAsset xml = Resources.Load<TextAsset>(PathDefine.mapCfgs);
        if (xml == null)
        {
            PECommon.Log("xml file:" + PathDefine.mapCfgs + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            //解析xml文件
            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;
            foreach (XmlElement element in nodLst)
            {
                if (element.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                else
                {
                    int id = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);
                    MapConfigs mc = new MapConfigs() { id = id, monsterLst = new List<MonsterData>(), };
                    foreach (XmlElement e in element.ChildNodes)
                    {
                        switch (e.Name)
                        {
                            case "mapName":
                                {
                                    mc.mapName = e.InnerText;
                                }
                                break;

                            case "sceneName":
                                {
                                    mc.sceneName = e.InnerText;
                                }
                                break;

                            case "power":
                                {
                                    mc.power = int.Parse(e.InnerText);
                                }
                                break;

                            case "mainCamPos":
                                {
                                    string[] temp = e.InnerText.Split(',');
                                    mc.mainCamPos = new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
                                }
                                break;

                            case "mainCamRote":
                                {
                                    string[] temp = e.InnerText.Split(',');
                                    mc.mainCamRote = new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
                                }
                                break;

                            case "playerBornPos":
                                {
                                    string[] temp = e.InnerText.Split(',');
                                    mc.playerBornPos = new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
                                }
                                break;

                            case "playerBornRote":
                                {
                                    string[] temp = e.InnerText.Split(',');
                                    mc.playerBornRote = new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
                                }
                                break;

                            case "monsterLst":
                                {
                                    string[] waveArr = e.InnerText.Split('#');
                                    for (int waveIndex = 0; waveIndex < waveArr.Length; waveIndex++)
                                    {
                                        if (waveIndex == 0)
                                        {
                                            continue;
                                        }
                                        string[] mIndexArr = waveArr[waveIndex].Split('|');
                                        for (int k = 0; k < mIndexArr.Length; k++)
                                        {
                                            if (k == 0)
                                            {
                                                continue;
                                            }
                                            string[] arr = mIndexArr[k].Split(',');
                                            MonsterData ml = new MonsterData
                                            {
                                                id = int.Parse(arr[0]),
                                                mWave = waveIndex,
                                                mIndex = k,
                                                mBornPos = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3])),
                                                mBornRote = new Vector3(0, float.Parse(arr[4]), 0),
                                                mCfg = GetMonsterInfo(int.Parse(arr[0])),
                                                mLevel = int.Parse(arr[5]),
                                            };
                                            mc.monsterLst.Add(ml);
                                        }
                                    }
                                }
                                break;
                            case "coin":
                                mc.coin = int.Parse(e.InnerText);
                                break;
                            case "exp":
                                mc.exp = int.Parse(e.InnerText);
                                break;
                            case "crystal":
                                mc.crystal = int.Parse(e.InnerText);
                                break;
                        }
                    }
                    mapDic.Add(id, mc);
                }
            }
        }
    }

    public MapConfigs GetMapInfo(int val)
    {
        MapConfigs mapCfg = null;
        if (mapDic.TryGetValue(val, out mapCfg))
        {
            return mapCfg;
        }
        return null;
    }
    #endregion

    #region  怪物配置

    Dictionary<int, MonsterConfigs> monsterDic = new Dictionary<int, MonsterConfigs>();
    public void InitMonster()
    {
        TextAsset xml = Resources.Load<TextAsset>(PathDefine.monsterCfgs);
        if (xml == null)
        {
            PECommon.Log("xml file" + PathDefine.monsterCfgs + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList lst = doc.SelectSingleNode("root").ChildNodes;
            foreach (XmlElement item in lst)
            {
                if (item.GetAttribute("ID") == null)
                {
                    continue;
                }
                else
                {
                    int id = Convert.ToInt32(item.GetAttributeNode("ID").InnerText);
                    MonsterConfigs mc = new MonsterConfigs { id = id,props = new BattleProps(), };

                    foreach (XmlElement e in item)
                    {
                        switch (e.Name)
                        {
                            case "mName":
                                mc.mName = e.InnerText;
                                break;
                            case "mType":
                                if (e.InnerText.Equals("1"))
                                {
                                    mc.mType = MonsterType.Normal;
                                }else if (e.InnerText.Equals("2"))
                                {
                                    mc.mType = MonsterType.Boss;
                                }
                                break;
                            case "isStop":
                                mc.isStop = int.Parse(e.InnerText) == 1;
                                break;
                            case "resPath":
                                mc.resPath = e.InnerText;
                                break;
                            case "skillID":
                                mc.skillID = int.Parse(e.InnerText);
                                break;
                            case "atkDis":
                                mc.atkDis = float.Parse(e.InnerText);
                                break;
                            case "hp":
                                mc.props.hp = int.Parse(e.InnerText);
                                break;
                            case "ad":
                                mc.props.ad = int.Parse(e.InnerText);
                                break;
                            case "ap":
                                mc.props.ap = int.Parse(e.InnerText);
                                break;
                            case "addef":
                                mc.props.addef = int.Parse(e.InnerText);
                                break;
                            case "apdef":
                                mc.props.apdef = int.Parse(e.InnerText);
                                break;
                            case "dodge":
                                mc.props.dodge = int.Parse(e.InnerText);
                                break;
                            case "pierce":
                                mc.props.pierce = int.Parse(e.InnerText);
                                break;
                            case "critical":
                                mc.props.critical = int.Parse(e.InnerText);
                                break;
                        }
                    }
                    monsterDic.Add(id, mc);
                }
            }
        }
    }

    public MonsterConfigs GetMonsterInfo(int val)
    {
        MonsterConfigs data = null;
        if (monsterDic.TryGetValue(val, out data))
        {
            return data;
        }
        return null;
    }

    #endregion

    #region 读取技能

    Dictionary<int, SkillConfigs> skillDic = new Dictionary<int, SkillConfigs>();
    public void InitSkill()
    {
        TextAsset xml = Resources.Load<TextAsset>(PathDefine.skillCfgs);
        if (xml == null)
        {
            PECommon.Log("xml file:" + PathDefine.skillCfgs + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList lst = doc.SelectSingleNode("root").ChildNodes;
            foreach (XmlElement item in lst)
            {
                if (item.GetAttribute("ID") == null)
                {
                    continue;
                }
                else
                {
                    int id = Convert.ToInt32(item.GetAttributeNode("ID").InnerText);
                    SkillConfigs sc = new SkillConfigs() { id = id, skillMoveLst = new List<int>(), skillActionLst = new List<int>(),skillDamageLst = new List<int>(), };
                    foreach (XmlElement e in item)
                    {
                        switch (e.Name)
                        {
                            case "skillName":
                                {
                                    sc.skillName = e.InnerText;
                                }
                                break;

                            case "skillTime":
                                {
                                    sc.skillTime = int.Parse(e.InnerText);
                                }
                                break;

                            case "cdTime":
                                {
                                    sc.cdTime = int.Parse(e.InnerText);
                                }
                                break;

                            case "aniAction":
                                {
                                    sc.aniAction = int.Parse(e.InnerText);
                                }
                                break;

                            case "fx":
                                {
                                    sc.fx = e.InnerText;
                                }
                                break;

                            case "isCombo":
                                {
                                    sc.isCombo = e.InnerText.Equals("1");
                                }
                                break;

                            case "isCollide":
                                {
                                    sc.isCollide = e.InnerText.Equals("1");
                                }
                                break;

                            case "isBreak":
                                {
                                    sc.isBreak = e.InnerText.Equals("1");
                                }
                                break;

                            case "dmgType":
                                {
                                    sc.dmgType = (DamageType)(int.Parse(e.InnerText));
                                }
                                break;
                            case "skillMoveLst":
                                string[] skMoveArr = e.InnerText.Split('|');
                                for (int j = 0; j < skMoveArr.Length; j++)
                                {
                                    if (skMoveArr[j] != "")
                                    {
                                        sc.skillMoveLst.Add(int.Parse(skMoveArr[j]));
                                    }
                                }
                                break;
                            case "skillActionLst":
                                string[] skaLst = e.InnerText.Split('|');
                                for (int j = 0; j < skaLst.Length; j++)
                                {
                                    if (skaLst[j] != "")
                                    {
                                        sc.skillActionLst.Add(int.Parse(skaLst[j]));
                                    }
                                }
                                break;
                            case "skillDamageLst":
                                string[] damageLst = e.InnerText.Split('|');
                                for (int j = 0; j < damageLst.Length; j++)
                                {
                                    if (damageLst[j] != "")
                                    {
                                        sc.skillDamageLst.Add(int.Parse(damageLst[j]));
                                    }
                                }
                                break;
                        }
                    }
                    skillDic.Add(id, sc);
                }
            }
        }
    }

    public SkillConfigs GetSkillInfo(int val)
    {
        SkillConfigs skillCfg = null;
        if (skillDic.TryGetValue(val, out skillCfg))
        {
            return skillCfg;
        }
        return null;
    }

    #endregion

    #region 读取技能移动

    Dictionary<int, SkillMoveConfigs> skillMoveDic = new Dictionary<int, SkillMoveConfigs>();
    public void InitSkillMove()
    {
        TextAsset xml = Resources.Load<TextAsset>(PathDefine.skillMoveCfgs);
        if (xml == null)
        {
            PECommon.Log("xml file:" + PathDefine.skillMoveCfgs + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList lst = doc.SelectSingleNode("root").ChildNodes;
            foreach (XmlElement item in lst)
            {
                if (item.GetAttribute("ID") == null)
                {
                    continue;
                }
                else
                {
                    int id = Convert.ToInt32(item.GetAttributeNode("ID").InnerText);
                    SkillMoveConfigs smc = new SkillMoveConfigs() { id = id };
                    foreach (XmlElement e in item)
                    {
                        switch (e.Name)
                        {
                            case "delayTime":
                                {
                                    smc.delayTime = int.Parse(e.InnerText);
                                }
                                break;

                            case "moveTime":
                                {
                                    smc.moveTime = int.Parse(e.InnerText);
                                }
                                break;

                            case "moveDis":
                                {
                                    smc.moveDis = float.Parse(e.InnerText);
                                }
                                break;
                        }
                    }
                    skillMoveDic.Add(id, smc);
                }
            }
        }
    }

    public SkillMoveConfigs GetSkillMoveInfo(int val)
    {
        SkillMoveConfigs skillMoveCfg = null;
        if (skillMoveDic.TryGetValue(val, out skillMoveCfg))
        {
            return skillMoveCfg;
        }
        return null;
    }
    #endregion
    #region 技能伤害和参数配置
    Dictionary<int, SkillActionConfigs> skillActionDic = new Dictionary<int, SkillActionConfigs>();
    public void InitSkillAction()
    {
        TextAsset xml = Resources.Load<TextAsset>(PathDefine.skillAcitonCfgs);
        if(xml == null)
        {
            PECommon.Log("file xml" + PathDefine.skillAcitonCfgs + "not exist", LogType.Error);
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);
            XmlNodeList lst = doc.SelectSingleNode("root").ChildNodes;

            foreach (XmlElement item in lst)
            {
                if(item.GetAttribute("ID") == null)
                {
                    continue;
                }
                else
                {
                    int id = Convert.ToInt32(item.GetAttributeNode("ID").InnerText);
                    SkillActionConfigs skaCfg = new SkillActionConfigs() {id =id };

                    foreach (XmlElement e in item)
                    {
                        switch (e.Name)
                        {
                            case "delayTime":
                                skaCfg.delayTime = int.Parse(e.InnerText);
                                break;
                            case "radius":
                                skaCfg.radius = float.Parse(e.InnerText);
                                break;
                            case "angle":
                                skaCfg.angle = int.Parse(e.InnerText);
                                break;
                        }
                    }
                    skillActionDic.Add(id, skaCfg);
                }
            }
        }
    }

    public SkillActionConfigs GetSkillAtionInfo(int id)
    {
        SkillActionConfigs skaCfg = null;
        if(skillActionDic.TryGetValue(id,out skaCfg))
        {
            return skaCfg;
        }
        return null;
    }
    #endregion

}