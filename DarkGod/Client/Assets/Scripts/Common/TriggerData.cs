/****************************************************
    文件：TriggerData.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/9/14 18:33:33
	功能：Nothing
*****************************************************/

using UnityEngine;

public class TriggerData : MonoBehaviour 
{
    public int BornIndex;
    public MapMgr mapMgr;

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(mapMgr != null)
            {
                mapMgr.TriggerExitMonsterBorn(this, BornIndex);
            }
        }
    }
}