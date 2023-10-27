/****************************************************
    文件：LoopDragonAni.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/12 20:49:55
	功能：龙的动画控制
*****************************************************/

using UnityEngine;

public class LoopDragonAni : MonoBehaviour 
{
    private Animation ani;

    private void Awake()
    {
        ani = GetComponent<Animation>();
    }

    private void Start()
    {
        if(ani != null)
        {
            InvokeRepeating("PlayDragonAni", 0, 20);
        }
    }

    public void PlayDragonAni()
    {
        if(ani != null)
        {
            ani.Play();
        }
    }
}