/****************************************************
    文件：SystemRoot.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/12 20:35:35
	功能：业务系统的基类
*****************************************************/

using UnityEngine;

public class SystemRoot : MonoBehaviour 
{
    protected AudioSvc auSvc = null;
    protected ResSvc resSvc = null;
    protected NetSvc netSvc = null;
    

    public virtual void InitSys()
    {
        auSvc = AudioSvc.Instance;
        resSvc = ResSvc.Instance;
        netSvc = NetSvc.Instance;
        
    }

}