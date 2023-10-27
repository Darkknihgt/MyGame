/****************************************************
    文件：WindowRoot.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/12 14:9:0
	功能：窗口脚本的基类
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;
using System;
using UnityEngine.EventSystems;

public class WindowRoot : MonoBehaviour 
{
    protected ResSvc resSvc = null;
    protected AudioSvc auSvc = null;
    protected NetSvc netSvc = null;
    protected TimerSvc timerSvc = null;
    //进行状态变化
    public void SetWndState(bool isActive = true)
    {
        if(gameObject.activeSelf != isActive)
        {
            SetActive(gameObject, isActive);
        }
        if(isActive == true)
        {
            InitWnd();
        }
        else
        {
            ClearWnd();
        }
    }

    //初始化
    protected virtual void InitWnd()
    {
        resSvc = ResSvc.Instance;
        auSvc = AudioSvc.Instance;
        netSvc = NetSvc.Instance;
        timerSvc = TimerSvc.Instance;
    }

    //结束窗口进行清理
    public virtual void ClearWnd()
    {
        resSvc = null;
        auSvc = null;
        netSvc = null;
        timerSvc = null;
    }

    #region Tool Functions
    protected void SetActive(GameObject go , bool isActive = true)
    {
        go.SetActive(isActive);
    }
    protected void SetActive(Transform trans,bool state = true)
    {
        trans.gameObject.SetActive(state);
    }
    protected void SetActive(RectTransform rectTrans,bool state = true)
    {
        rectTrans.gameObject.SetActive(state);
    }
    protected void SetActive(Text txt,bool state = true)
    {
        txt.transform.gameObject.SetActive(state);
    }



    protected void SetText(Text txt,string context = "")
    {
        txt.text = context;
    }
    protected void SetText(Transform trans,int num = 0)
    {
        SetText(trans.GetComponent<Text>(), num);
    }
    protected void SetText(Text txt,int num)
    {
        SetText(txt, num.ToString());
    }
    protected void SetText(Transform trans,string context = "")
    {
        SetText(trans.GetComponent<Text>(), context);
    }

    /// <summary>
    /// 获取组件或添加组件工具函数
    /// </summary>
    protected T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if(component == null)
        {
            component = go.AddComponent<T>();
        }
        return component;
    }
    /// <summary>
    /// 通过父物体的trans和查询物体名字获取查询物体的trans
    /// </summary>
    /// <param 父物体trans="trans"></param>
    /// <param 查询目标名字="name"></param>
    /// <returns></returns>
    protected Transform GetItemTrans(Transform trans,string name)
    {
        if(trans != null)
        {
            return trans.Find(name);
        }
        else
        {
            return transform.Find(name);
        }
    }

    #endregion

    #region Click Evts

    protected void OnClick(GameObject go,Action<object> cb,object arg)   //点击事件，传入对象，事件清单函数
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);   //先给该对象添加一个事件接听脚本
        listener.onClick = cb;    //给onclick事件清单添加内容
        listener.args = arg;
    }

    protected void OnClickDown(GameObject go, Action<PointerEventData> cb)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onClickDown = cb;
    }

    protected void OnClickUp(GameObject go, Action<PointerEventData> cb)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onClickUp = cb;
    }

    protected void OnClickDrag(GameObject go, Action<PointerEventData> cb)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onClickDrag = cb;
    }
    #endregion

    protected void SetSprite(Image img,string path)
    {
        Sprite sp = resSvc.LoadSprite(path, true);
        img.sprite = sp;
    }
}