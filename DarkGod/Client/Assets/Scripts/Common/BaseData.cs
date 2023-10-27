/****************************************************
    文件：PathDefine.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/14 13:0:30
	功能：基础配置类
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public class SkillMoveConfigs : BaseData<SkillMoveConfigs>
{
    public int moveTime;
    public float moveDis;
    public int delayTime;
}

public class SkillActionConfigs : BaseData<SkillActionConfigs>
{
    public int delayTime;
    public float radius;
    public int angle;
}

public class SkillConfigs : BaseData<SkillConfigs>
{
    public string skillName;
    public int cdTime;
    public int skillTime;
    public int aniAction;
    public string fx;
    public bool isCombo;
    public bool isCollide;
    public bool isBreak;

    public DamageType dmgType;
    public List<int> skillMoveLst;
    public List<int> skillActionLst;
    public List<int> skillDamageLst;
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
    public int npcID;
    public string dilogArr;
    public int actID;
    public int coin;
    public int exp;
}

public class MonsterConfigs : BaseData<MonsterConfigs>
{
    public string mName;
    public MonsterType mType;//1:普通怪物 2：Boss怪物
    public bool isStop;
    public string resPath;
    public BattleProps props;
    public int skillID;
    public float atkDis;
}

public class MonsterData : BaseData<MonsterData>
{
    public int mWave;
    public int mIndex;
    public int mLevel;
    public MonsterConfigs mCfg;
    public Vector3 mBornPos;
    public Vector3 mBornRote;
}

public class MapConfigs : BaseData<MapConfigs>
{
    public string mapName;
    public string sceneName;
    public int power;
    public Vector3 mainCamPos;
    public Vector3 mainCamRote;
    public Vector3 playerBornPos;
    public Vector3 playerBornRote;

    public List<MonsterData> monsterLst;

    public int coin;
    public int exp;
    public int crystal;
}


public class TaskRewardCfg : BaseData<TaskRewardCfg>
{
    public string taskName;
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

public class BattleProps //战斗计算用属性
{
    public int hp;
    public int ad;
    public int ap;
    public int addef;
    public int apdef;
    public int dodge;
    public int pierce;
    public int critical;
}
