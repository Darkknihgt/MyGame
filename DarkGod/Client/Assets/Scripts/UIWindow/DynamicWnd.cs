/****************************************************
    文件：DynamicWnd.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/12 21:52:12
	功能：控制动态UI界面
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicWnd : WindowRoot
{
    public Animation tipsAni;
    public Animation selfDodgeAnim;
    public Text txtTips;
    public Transform hpItemRoot;

    private Dictionary<string, ItemEntityHP> mHPdic = new Dictionary<string, ItemEntityHP>();

    private bool isTipsShow = false;

    protected override void InitWnd()
    {
        base.InitWnd();

        SetActive(txtTips, false);
    }

    #region tips相关
    Queue<string> tipsQue = new Queue<string>();
    public void AddTips(string tips)
    {
        //加入锁，并入队
        lock (tipsQue)
        {
            tipsQue.Enqueue(tips);
        }
    }

    private void Update()
    {
        if(tipsQue.Count > 0 && isTipsShow == false)
        {
            lock (tipsQue)
            {
                string tips = tipsQue.Dequeue();
                isTipsShow = true;               
                SetTips(tips);
            }
        }
    }

    /// <summary>
    /// 对tips进行设置
    /// </summary>
    /// <param name="tips"></param>
    public void SetTips(string tips)
    {
        SetActive(txtTips, true);
        SetText(txtTips, tips);

        //获取动画片段
        AnimationClip clip = tipsAni.GetClip("TipsShowAnim");
        tipsAni.Play();
   
        //协程延迟关闭激活，等待动画播完关闭组件
        StartCoroutine(AniPlayDone(clip.length,() => {
            SetActive(txtTips, false);
            isTipsShow = false;          
        }));
    }

    private IEnumerator AniPlayDone(float sec, Action ac)
    {
        yield return new WaitForSeconds(sec);
        if(ac != null)
        {
            ac();
        }
    }

    #endregion


    /// <summary>
    /// 该方法是给加载怪物时调用,设置怪物的血条信息
    /// </summary>
    /// <param name="mName"></param>
    /// <param name="hp"></param>
    public void AddHPItemInfo(string mName, Transform HpRoot,int hp)
    {
        ItemEntityHP item = null;
        if(mHPdic.TryGetValue(mName,out item))
        {
            return;
        }
        else
        {
            GameObject go = resSvc.LoadPrefabs(PathDefine.MonsterHPItemPrefab, true); //从资源中加载出来该预制体
            go.transform.SetParent(hpItemRoot);
            go.transform.localPosition = new Vector3(-1000, 0, 0);
            ItemEntityHP ieh = go.GetComponent<ItemEntityHP>();
            ieh.InitItemInfo(HpRoot, hp);
            mHPdic.Add(mName, ieh);
        }
    }

    public void RemoveItemInfo(string mName)
    {
        ItemEntityHP item = null;
        if(mHPdic.TryGetValue(mName,out item))
        {
            Destroy(item.gameObject);
            mHPdic.Remove(mName);
        }
    }

    public void RemoveHPItemInfo()
    {
        foreach(var item in mHPdic)
        {
            Destroy(item.Value.gameObject);
        }
        mHPdic.Clear();
    }

    #region 外部调用接口，用来弹出伤害等效果

    public void SetDodge(string mName)
    {
        ItemEntityHP ieh = null;
        if(mHPdic.TryGetValue(mName,out ieh))
        {
            ieh.SetDodge();
        }
    }

    public void SetCritical(string mName,int critical)
    {

        ItemEntityHP ieh = null;
        if (mHPdic.TryGetValue(mName, out ieh))
        {
            ieh.SetCritical(critical);
        }
    }

    public void SetHurt(string mName,int hurt)
    {
        ItemEntityHP ieh = null;
        if (mHPdic.TryGetValue(mName, out ieh))
        {
            ieh.SetHP(hurt);
        }
    }

    public void SetHp(string mName, int oldHP,int newHP)
    {
        ItemEntityHP ieh = null;
        if (mHPdic.TryGetValue(mName, out ieh))
        {
            ieh.HPChange(oldHP, newHP);
        }
    }

    public void SetSelfDodge()
    {
        selfDodgeAnim.Stop();
        selfDodgeAnim.Play();
    }
    #endregion
}