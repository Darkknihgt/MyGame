/****************************************************
    文件：ItemEntityHP.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/8/2 23:2:31
	功能：处理血条等特效
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class ItemEntityHP : MonoBehaviour
{
    public Image fgGray;
    public Image fgRed;

    public Animation CriticalAniShow;
    public Text txtCritical;

    public Animation dodgeAniShow;
    public Text txtDodge;

    public Animation HPAniShow;
    public Text txtHP;

    private int hpVal;
    private RectTransform rect;
    private Transform mWorldPos;
    private float ScreenRate = 1.0f * Constants.ScreenStandardHeight / Screen.height;

    private float currentPrg;
    private float targetPrg;

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SetCritical(777);
        //    SetHP(777);
        //}

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    SetDodge();
        //}
        Vector3 mScreenPos = Camera.main.WorldToScreenPoint(mWorldPos.position);
        rect.anchoredPosition = mScreenPos * ScreenRate;

        HPMixBlend();

    }

    private void HPMixBlend()
    {


        if (Mathf.Abs(currentPrg - targetPrg) < Constants.HPChangeSpeed)
        {
            currentPrg = targetPrg;
        }
        else if (currentPrg > targetPrg)
        {
            currentPrg -= Constants.HPChangeSpeed * Time.deltaTime;
        }
        else
        {
            currentPrg += Constants.HPChangeSpeed * Time.deltaTime;
        }


        fgGray.fillAmount = currentPrg;
    }


    public void InitItemInfo(Transform mPos, int hp)
    {
        rect = transform.GetComponent<RectTransform>();
        mWorldPos = mPos;

        hpVal = hp;
        fgGray.fillAmount = 1;
        fgRed.fillAmount = 1;
    }

    

    public void SetCritical(int critical)
    {
        //为了让特效表现为技能打出伤害，就立即显示，所以会先终止上个特效
        CriticalAniShow.Stop();
        txtCritical.text = "暴击 " + critical;
        CriticalAniShow.Play();
    }

    public void SetDodge()
    {
        //为了让特效表现为技能打出伤害，就立即显示，所以会先终止上个特效
        dodgeAniShow.Stop();
        txtDodge.text = "闪避";
        dodgeAniShow.Play();
    }

    public void SetHP(int minusHp)
    {
        //为了让特效表现为技能打出伤害，就立即显示，所以会先终止上个特效
        HPAniShow.Stop();
        txtHP.text = "-" + minusHp;
        HPAniShow.Play();
    }

    public void HPChange(int oldHP, int newHP)
    {
        currentPrg = oldHP * 1.0f / hpVal;
        targetPrg = newHP * 1.0f / hpVal;

        fgRed.fillAmount = targetPrg;
    }
}