/****************************************************
    文件：TestPlayer.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/26 22:35:31
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : Controller 
{
   // public CharacterController pContr;
    public GameObject DaggerSkill1fx;

    private Transform camTrans;
   

    private Vector3 offSet;

    //Idle and walk
    private float currentBlend;
    private float targetBlend;



    private void Start()
    {
        camTrans = Camera.main.transform;
        offSet = transform.position - camTrans.position;
        currentBlend = 0;
    }

    private void Update()
    {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 moveDir = new Vector2(h, v);
        if (moveDir != Vector2.zero)
        {
            Dir = moveDir;
            targetBlend = Constants.walkAnim;
            AnimTrans();
        }
        else
        {
            Dir = Vector2.zero;
            targetBlend = Constants.idleAnim;
            AnimTrans();
        }
        AnimTrans();
        if (isMove)
        {
            SetDir();
            SetMove();
            SetCam();
        }
        if (currentBlend != targetBlend)
        {
            AnimLerpFunc();
        }
    }

    /// <summary>
    /// 设置移动方向
    /// </summary>
    private void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1)) + camTrans.eulerAngles.y;
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        pContr.transform.eulerAngles = eulerAngle;
    }

    /// <summary>
    /// 设置移动
    /// </summary>
    private void SetMove()
    {
        pContr.Move(transform.forward * Time.deltaTime * Constants.playerSpeed);
    }

    /// <summary>
    /// 设置摄像机跟随
    /// </summary>
    public void SetCam()
    {
        if (camTrans != null)
        {
            camTrans.position = transform.position - offSet;
        }
    }

    /// <summary>
    /// 动画转换
    /// </summary>
    /// <param name="blend"></param>
    public void AnimTrans()
    {
        animC.SetFloat("Blend", currentBlend);
    }

    public override void SetBlend(float blend)
    {
        targetBlend = blend;
    }

    /// <summary>
    /// belnd值过度
    /// </summary>
    private void AnimLerpFunc()
    {
        //当当前混合接近目标混合
        if (Mathf.Abs(currentBlend - targetBlend) < Constants.transSpeed * Time.deltaTime)
        {
            currentBlend = targetBlend;
            //Debug.Log(currentBlend + "   " + targetBlend);
        }
        else if (currentBlend < targetBlend)
        {
            currentBlend += Constants.transSpeed * Time.deltaTime;
        }
        else
        {
            currentBlend -= Constants.transSpeed * Time.deltaTime;
        }
        animC.SetFloat("Blend", currentBlend);
    }

    public void ClickSkill1()
    {
        animC.SetInteger("Action", 1);
        DaggerSkill1fx.SetActive(true);
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.8f);

        DaggerSkill1fx.SetActive(false);
        animC.SetInteger("Action", -1);
    }
}