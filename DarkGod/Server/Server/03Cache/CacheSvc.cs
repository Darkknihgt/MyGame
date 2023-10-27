/****************************************************
    文件：ServerSession.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/15 23:5:29
	功能：数据管理
*****************************************************/

using PEProtocol;
using System;
using System.Collections.Generic;



public class CacheSvc
{
    private static CacheSvc instance = null;
    public static CacheSvc Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new CacheSvc();
            }
            return instance;
        }
    }

    DBMgr dbMgr = null;
    public void InitCache()
    {
        dbMgr = DBMgr.Instance;
        PECommon.Log("InitCache Done");
    }

    private Dictionary<string, ServerSession> onLineAcctDic = new Dictionary<string, ServerSession>();
    private Dictionary<ServerSession, PlayerData> onLineSessionDic = new Dictionary<ServerSession, PlayerData>();
    public bool IsAcctOnLine(string acct)
    {
        return onLineAcctDic.ContainsKey(acct);
    }

    /// <summary>
    /// 根据账号密码返回对应账号数据，密码错误返回null，账号不存在则默认创新账号
    /// </summary>
    public PlayerData GetPlayerData(string acct,string pass)
    {
        //TODO
        //从数据库中读取玩家消息
        PlayerData pd = dbMgr.QueryPlayerData(acct, pass);
        //当密码不正确时，返回null
        return pd;
    }

    /// <summary>
    /// 账号上线，缓存数据
    /// </summary>    
    public void AcctOnline(string acct,ServerSession session,PlayerData playerData)
    {
        onLineAcctDic.Add(acct, session);
        onLineSessionDic.Add(session, playerData);
    }

    /// <summary>
    /// 确定数据库中是否存在名字
    /// </summary>
    /// <param name="reqRename"></param>
    /// <returns></returns>
    public bool IsNameExist(ReqRename reqRename)
    {
        return dbMgr.QueryNameData(reqRename.name);
    }

    /// <summary>
    /// 通过session调取玩家信息，第一次是修改名字时候使用
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public PlayerData GetPlayerDataBySession(ServerSession session)
    {
        PlayerData playerdata = null;
        onLineSessionDic.TryGetValue(session, out playerdata);
        return playerdata;
    }

    /// <summary>
    /// 获取所有在线玩家的ServerSession
    /// </summary>
    /// <returns></returns>
    public List<ServerSession> GetPOnlineServerSessions()
    {
        List<ServerSession> lst = new List<ServerSession>();
        foreach (var item in onLineSessionDic)
        {
            lst.Add(item.Key);
        }
        return lst;
    }

    public Dictionary<ServerSession,PlayerData> GetOnlineCache()
    {
        return onLineSessionDic;
    }

    /// <summary>
    /// 更新缓存中的玩家信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pd"></param>
    /// <returns></returns>
    public bool UpdatePlayerData(int id,PlayerData pd)
    {
        return dbMgr.UpdatePlayerData(id,pd);
    }

    /// <summary>
    /// 清空字典，下线
    /// </summary>
    /// <param name="session"></param>
    public void AcctOffLine(ServerSession session)
    {
        foreach (var item in onLineAcctDic)
        {
            if(item.Value == session)
            {
                onLineAcctDic.Remove(item.Key);
                break;
            }
        }

        bool succ = onLineSessionDic.Remove(session);
        PECommon.Log("Offline Ressult:" + succ +",SessionID:"+session.sessionID);
    }


    public ServerSession GetOnlineServersessionByID(int id)
    {
        ServerSession session = null;
        foreach (var item in onLineSessionDic)
        {
            if(item.Value.id == id)
            {
                session = item.Key;
                break;
            }
        }
        return session;
    }
}

