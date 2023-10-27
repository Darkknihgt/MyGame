/****************************************************
    文件：ServerSession.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/15 23:5:29
	功能：数据库层
*****************************************************/

using MySql.Data.MySqlClient;
using PEProtocol;
using System;
using System.Collections.Generic;



public class DBMgr
{
    private static DBMgr instance = null;
    public static DBMgr Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new DBMgr();
            }
            return instance;
        }
    }
    
    private MySqlConnection conn;
    public void InitDBmgr()
    {
        conn = new MySqlConnection("server=localhost;User Id=root;password=;Database=darkgod;Charset=utf8");
        conn.Open();
        PECommon.Log("DBMgr Init Done");

        //QueryPlayerData("1234", "2222");
    }

    public PlayerData QueryPlayerData(string acct,string pass)
    {
        PlayerData playerData = null;
        MySqlDataReader reader = null;
        bool isNew = true;

        MySqlCommand cmd = new MySqlCommand("select * from account where acct =@acct", conn);
        cmd.Parameters.AddWithValue("acct", acct);

        try
        {
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                isNew = false;
                string _pass = reader.GetString("pass");
                if (_pass.Equals(pass))
                {
                    //密码匹配,返回玩家数据
                    playerData = new PlayerData
                    {
                        id = reader.GetInt32("id"),
                        name = reader.GetString("name"),
                        lv = reader.GetInt32("level"),
                        exp = reader.GetInt32("exp"),
                        power = reader.GetInt32("power"),
                        coin = reader.GetInt32("coin"),
                        diamond = reader.GetInt32("diamond"),
                        crystal = reader.GetInt32("crystal"),

                        hp = reader.GetInt32("hp"),
                        ad = reader.GetInt32("ad"),
                        ap = reader.GetInt32("ap"),
                        addef = reader.GetInt32("addef"),
                        apdef = reader.GetInt32("apdef"),
                        dodge = reader.GetInt32("dodge"),
                        pierce = reader.GetInt32("pierce"),
                        critical = reader.GetInt32("critical"),

                        guideid = reader.GetInt32("guideid"),
                        time = reader.GetInt64("time"),
                        fuben = reader.GetInt32("fuben"),
                        //TODD
                    };
                    #region 读取strongArr信息
                    //读取数据库中的strongArr信息
                    string[] strongStrArr = reader.GetString("strong").Split('#');  //将字符串信息进行分割

                    int[] _strongStrArr = new int[6]; //创建新的int数组用来存储转化后的信息
                    for(int i = 0;i < strongStrArr.Length; i++)
                    {
                        //当i的时候为空时，跳过
                        if(strongStrArr[i] == "")
                        {
                            continue;
                        }
                        if(int.TryParse(strongStrArr[i],out int starLv))
                        {
                            _strongStrArr[i] = starLv;
                        }
                        else //当转换失败的时候
                        {
                            PECommon.Log("Parse Strong Data Error", LogType.Error);
                        }
                    }
                    playerData.strongArr = _strongStrArr;
                    #endregion

                    #region 读取TaskArr中数据信息

                    string[] taskStrArr = reader.GetString("task").Split('#');
                    playerData.taskArr = new string[6];
                    for (int i = 0; i < taskStrArr.Length-1; i++)
                    {
                        if(taskStrArr[i] == "")
                        {
                            continue;
                        }else if(taskStrArr[i].Length < 5)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            playerData.taskArr[i] = taskStrArr[i];
                        }
                    }

                    #endregion
                }
            }
        }
        catch (Exception e)
        {
            PECommon.Log("Query PlayerData By Acct&Pass Error:" + e, LogType.Error);
        }
        finally
        {
            if(reader != null)
            {
                reader.Close();
            }
            if (isNew)
            {
                //不存在账号数据，创建新的默认账号数据，并返回
                playerData = new PlayerData
                {
                    id = -1,
                    name = "",
                    lv = 1,
                    exp = 0,
                    power = 150,
                    coin = 5000,
                    diamond = 500,
                    crystal = 500,

                    hp = 2000,
                    ad = 275,
                    ap = 265,
                    addef = 67,
                    apdef = 43,
                    dodge = 7,
                    pierce = 5,
                    critical = 2,

                    guideid = 1001,
                    strongArr = new int[6], //默认全部为0
                    time = TimerSvc.Instance.GetNowTime(),
                    taskArr = new string[6],
                    fuben = 10001,
                    //TODO
                };
                //初始化任务奖励数据  1|0|0#2|0|0#3|0|0#4|0|0#5|0|0#6|0|0
                for (int i = 0; i < playerData.taskArr.Length; i++)
                {
                    playerData.taskArr[i] = (i + 1) + "|0|0";
                }

                playerData.id = InsertNewAcctData(acct, pass, playerData);
            }
        }

        return playerData;
    }

    private int InsertNewAcctData(string acct,string pass,PlayerData pd)
    {
        int id = -1;
        try
        {
            MySqlCommand cmd = new MySqlCommand("insert into account set acct=@acct,pass=@pass,name=@name,level=@level," +
                "exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal,hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef," +
                "dodge=@dodge,pierce=@pierce,critical=@critical,guideid =@guideid,strong = @strong,time=@time,task=@task,fuben=@fuben", conn);
            cmd.Parameters.AddWithValue("acct", acct);
            cmd.Parameters.AddWithValue("pass", pass);
            cmd.Parameters.AddWithValue("name", pd.name);
            cmd.Parameters.AddWithValue("level", pd.lv);
            cmd.Parameters.AddWithValue("exp", pd.exp);
            cmd.Parameters.AddWithValue("power", pd.power);
            cmd.Parameters.AddWithValue("coin", pd.coin);
            cmd.Parameters.AddWithValue("diamond", pd.diamond);
            cmd.Parameters.AddWithValue("crystal", pd.crystal);

            cmd.Parameters.AddWithValue("hp", pd.hp);
            cmd.Parameters.AddWithValue("ad", pd.ad);
            cmd.Parameters.AddWithValue("ap", pd.ap);
            cmd.Parameters.AddWithValue("addef", pd.addef);
            cmd.Parameters.AddWithValue("apdef", pd.apdef);
            cmd.Parameters.AddWithValue("dodge", pd.dodge);
            cmd.Parameters.AddWithValue("pierce", pd.pierce);
            cmd.Parameters.AddWithValue("critical", pd.critical);

            cmd.Parameters.AddWithValue("guideid", pd.guideid);
            cmd.Parameters.AddWithValue("time", pd.time);
            cmd.Parameters.AddWithValue("fuben", pd.fuben);
            #region convert array to string
            string strongInfo = null;
            for (int i = 0; i < pd.strongArr.Length; i++)
            {
                strongInfo += pd.strongArr[i].ToString();
                strongInfo += "#";
            }
            cmd.Parameters.AddWithValue("strong", strongInfo);
            #endregion

            #region task data
            string taskInfo = null;
            for(int i = 0; i < pd.taskArr.Length; i++)
            {
                taskInfo += pd.taskArr[i].ToString();
                taskInfo += "#";
            }
            cmd.Parameters.AddWithValue("task", taskInfo);
            #endregion
            //TODO
            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId;
        }
        catch(Exception e)
        {
            PECommon.Log("Insert Error:" + e, LogType.Error);
        }
        return id;
    }

    /// <summary>
    /// 从数据库中查询名字
    /// </summary>
    /// <param name="_name"></param>
    /// <returns></returns>
    public bool QueryNameData(string _name)
    {
        bool exist = false;
        MySqlDataReader reader = null;

        try
        {
            MySqlCommand cmd = new MySqlCommand("select *from account where name=@name", conn);
            cmd.Parameters.AddWithValue("name", _name);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                exist = true;
            }
            if(reader != null)
            {
                reader.Close();
            }
        }
        catch(Exception e)
        {
            PECommon.Log("Query Name Error:" + e, LogType.Error);
        }

        return exist;
    }

    public bool UpdatePlayerData(int id,PlayerData pd)
    {
        try
        {
            MySqlCommand cmd = new MySqlCommand("update account set name=@name,level=@level," +
                "exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal,hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef," +
                "dodge=@dodge,pierce=@pierce,critical=@critical,guideid=@guideid,strong=@strong,time=@time,task=@task,fuben=@fuben where id=@id", conn);

            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("name", pd.name);
            cmd.Parameters.AddWithValue("level", pd.lv);
            cmd.Parameters.AddWithValue("exp", pd.exp);
            cmd.Parameters.AddWithValue("power", pd.power);
            cmd.Parameters.AddWithValue("coin", pd.coin);
            cmd.Parameters.AddWithValue("diamond", pd.diamond);
            cmd.Parameters.AddWithValue("crystal", pd.crystal);

            cmd.Parameters.AddWithValue("hp", pd.hp);
            cmd.Parameters.AddWithValue("ad", pd.ad);
            cmd.Parameters.AddWithValue("ap", pd.ap);
            cmd.Parameters.AddWithValue("addef", pd.addef);
            cmd.Parameters.AddWithValue("apdef", pd.apdef);
            cmd.Parameters.AddWithValue("dodge", pd.dodge);
            cmd.Parameters.AddWithValue("pierce", pd.pierce);
            cmd.Parameters.AddWithValue("critical", pd.critical);

            cmd.Parameters.AddWithValue("guideid", pd.guideid);
            cmd.Parameters.AddWithValue("time", pd.time);
            cmd.Parameters.AddWithValue("fuben", pd.fuben);

            #region convert array to string
            string strongInfo = null;
            for(int i = 0;i < pd.strongArr.Length; i++)
            {
                strongInfo += pd.strongArr[i].ToString();
                strongInfo += "#";
            }
            cmd.Parameters.AddWithValue("strong", strongInfo);
            #endregion
            #region
            string taskInfo = null;
            for(int i = 0; i < pd.taskArr.Length; i++)
            {
                taskInfo += pd.taskArr[i].ToString();
                taskInfo += "#";
            }
            cmd.Parameters.AddWithValue("task", taskInfo);
            #endregion

            cmd.ExecuteNonQuery();
        }
        catch(Exception e)
        {
            PECommon.Log("Update PlayerData Error:" + e, LogType.Error);
            return false;
        }
        return true;
    }
}

