/****************************************************
    文件：LoginSys.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/15 23:5:29
	功能：配置数据服务
*****************************************************/

using System;
using System.Collections.Generic;
using System.Xml;

public class CfgSvc
{
    private static CfgSvc instance = null;
    public static CfgSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CfgSvc();
            }
            return instance;
        }
    }

    public void InitCfgSvc()
    {
        InitGuideCfg();
        InitStrong();
        InitTaskRewardCfg();
        InitMapConfigs();
        PECommon.Log("CfgSvc init Done");
    }

    #region 读取引导数据
    Dictionary<int, GuideConfigs> guideDic1 = new Dictionary<int, GuideConfigs>();
    public void InitGuideCfg()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(@"D:\unitystudy\unitytest\DarkGod\Client\Assets\Resources\ResCfg\guide.xml");

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
                guideDic1.Add(id, mc);
            }

        }
        PECommon.Log("GuideCfg init Done");

        #endregion
    }
    public GuideConfigs GetGuideDataByID(int id)
    {
        GuideConfigs content = null;
        if (guideDic1.TryGetValue(id, out content))
        {
            return content;
        }
        return null;
    }

    #region 强化配置读取

    //字典用来存储读取的数据配置

    private Dictionary<int, Dictionary<int, StrongConfigs>> strongDic = new Dictionary<int, Dictionary<int, StrongConfigs>>();
    public void InitStrong()
    {

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(@"D:\unitystudy\unitytest\DarkGod\Client\Assets\Resources\ResCfg\strong.xml");

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
        PECommon.Log("StrongCfg init Done");

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
    #endregion

    #region 任务奖励数据读取

    Dictionary<int, TaskRewardCfg> taskRewardDic = new Dictionary<int, TaskRewardCfg>(); //用来存放任务奖励数据
    public void InitTaskRewardCfg()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(@"D:\unitystudy\unitytest\DarkGod\Client\Assets\Resources\ResCfg\taskreward.xml");

         XmlNodeList nodLst = xmlDoc.SelectSingleNode("root").ChildNodes; //获取root下所有的子节点表列
        foreach(XmlElement element in nodLst)
        {
            if (element.GetAttributeNode("ID") == null)
            {
                continue;
            }
            else
            {
                int id = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);
                TaskRewardCfg taskCfg = new TaskRewardCfg() { id = id };
                foreach(XmlElement item in element)
                {
                    switch (item.Name)
                    {
                        case "coin":
                            taskCfg.coin = int.Parse(item.InnerText);
                            break;
                        case "exp":
                            taskCfg.exp = int.Parse(item.InnerText);
                            break;
                        case "count":
                            taskCfg.count = int.Parse(item.InnerText);
                            break;
                    }
                }
                taskRewardDic.Add(id, taskCfg);
            }
        }
        PECommon.Log("TaskRewardCfg init Done");
    }

    public TaskRewardCfg GetTaskRewardCfgByID(int id)
    {
        TaskRewardCfg content = null;
        if (taskRewardDic.TryGetValue(id, out content))
        {
            return content;
        }
        return null;
    }

    #endregion

    #region 解析地图

    Dictionary<int, MapConfigs> mapDic = new Dictionary<int, MapConfigs>();
    public void InitMapConfigs()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(@"D:\unitystudy\unitytest\DarkGod\Client\Assets\Resources\ResCfg\map1.xml");
        XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

        foreach (XmlElement item in nodLst)
        {
            if (item.GetAttributeNode("ID") == null)
            {
                continue;
            }
            else
            {
                int id = Convert.ToInt32(item.GetAttributeNode("ID").InnerText);
                MapConfigs mapCfg = new MapConfigs() { id = id };
                foreach (XmlElement element in item)
                {
                    switch (element.Name)
                    {
                        case "power":
                            mapCfg.power = int.Parse(element.InnerText);
                            break;
                        case "coin":
                            mapCfg.coin = int.Parse(element.InnerText);
                            break;
                        case "exp":
                            mapCfg.exp = int.Parse(element.InnerText);
                            break;
                        case "crystal":
                            mapCfg.crystal = int.Parse(element.InnerText);
                            break;
                    }
                }
                mapDic.Add(id, mapCfg);
            }
        }
    }

    public MapConfigs GetMapCfg(int fbid)
    {
        MapConfigs mapCfg = null;
        if(mapDic.TryGetValue(fbid,out mapCfg))
        {
            return mapCfg;
        }
        return null;
    }

    #endregion  
}


public class MapConfigs : BaseData<MapConfigs>
{
    public int power;
    public int coin;
    public int exp;
    public int crystal;
}
public class StrongConfigs : BaseData<StrongConfigs>
{
    public int pos;
    public int starlv;
    public int addhp;
    public int addhurt;
    public int adddef;
    public int minlv;
    public int coin;
    public int crystal;
}

public class GuideConfigs : BaseData<GuideConfigs>
{
    public int coin;
    public int exp;
}

public class TaskRewardCfg : BaseData<TaskRewardCfg>
{
    public int count;
    public int coin;
    public int exp;
}

public class TaskRewardData : BaseData<TaskRewardData>
{
    public int prgs;
    public bool taked;
}

public class BaseData<T>
{
    public int id;
}

