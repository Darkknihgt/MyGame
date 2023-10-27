/****************************************************
    文件：LoadingWnd.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/12 0:17:26
	功能：绑定加载界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class LoadingWnd : WindowRoot
{
    //绑定加载页面各个参数
    public Text txtTips;
    public Image imageFg;
    public Image imgPoint;
    public Text txtPrg;
    //获取进度条的宽度
    private float fgWidth;

    //对参数进行值的初始化
    protected override void InitWnd()
    {
        base.InitWnd();

        fgWidth = imageFg.GetComponent<RectTransform>().sizeDelta.x;

        SetText(txtTips, "这是一条游戏的Tips");
        SetText(txtPrg, "0%");
        imageFg.fillAmount = 0;
        imgPoint.transform.localPosition = new Vector3(-545,0,0);

        //Debug.Log(imgPoint.GetComponent<RectTransform>().anchoredPosition);
        //Debug.Log(imgPoint.transform.localPosition);
    }

    //对参数进行值的变化
    public void SetProgress(float prg)
    {
        SetText(txtPrg,(int)(prg *100) + "%");
        imageFg.fillAmount = prg;

        float posX = prg * fgWidth - 545;
        imgPoint.transform.localPosition = new Vector3(posX, 0, 0);
        //imgPoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, 0);
    }
}