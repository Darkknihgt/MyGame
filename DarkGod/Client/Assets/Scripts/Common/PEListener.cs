/****************************************************
    文件：LoginSys.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/11 23:5:29
	功能：监听组件
*****************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PEListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler,IPointerClickHandler
{
    public Action<PointerEventData> onClickDown;
    public Action<PointerEventData> onClickUp;
    public Action<PointerEventData> onClickDrag;
    public Action<object> onClick; //创建点击清单

    public object args;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(onClick != null)  //只有当清单不为空的时候，才将监听到的eventData数据传入到清单中，并触发清单的函数
        {
            onClick(args);
        }
    }
    /// <summary>
    /// 当按下时触发
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if(onClickDown != null)
        {
        onClickDown(eventData);
        }
    }
    /// <summary>
    /// 当拖拽时触发
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if(onClickDrag != null)
        {
            onClickDrag(eventData);
        }
    }
    /// <summary>
    /// 当抬起时触发
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (onClickUp != null)
        {
            onClickUp(eventData);
        }
    }

}

