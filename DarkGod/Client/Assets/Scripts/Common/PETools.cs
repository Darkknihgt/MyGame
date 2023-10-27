/****************************************************
    文件：PETools.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/14 13:50:17
	功能：工具类
*****************************************************/

using UnityEngine;

public class PETools 
{
    public static int RDInt(int min,int max, System.Random rd = null)
    {
        if(rd == null)
        {
            rd = new System.Random();
        }

        return rd.Next(min, max + 1);
        
    }
}