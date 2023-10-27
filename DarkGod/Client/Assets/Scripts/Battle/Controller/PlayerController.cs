/****************************************************
    文件：PlayerController.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/20 23:24:34
	功能：玩家行为控制
*****************************************************/

using UnityEngine;

public class PlayerController : Controller
{
    
    
    

    
    private Vector3 offSet;

    //Idle and walk
    private float currentBlend;
    private float targetBlend;

    public GameObject daggerSK1fx;
    public GameObject daggerSK2fx;
    public GameObject daggerSK3fx;

    public GameObject daggeratkfx1;
    public GameObject daggeratkfx2;
    public GameObject daggeratkfx3;
    public GameObject daggeratkfx4;
    public GameObject daggeratkfx5;

    public override void Init()
    {
        base.Init();

        if(daggerSK1fx != null)
        {
        fxDic.Add(daggerSK1fx.name, daggerSK1fx);
        }
        if (daggerSK2fx != null)
        {
            fxDic.Add(daggerSK2fx.name, daggerSK2fx);
        }
        if (daggerSK3fx != null)
        {
            fxDic.Add(daggerSK3fx.name, daggerSK3fx);
        }

        if (daggeratkfx1 != null)
        {
            fxDic.Add(daggeratkfx1.name, daggeratkfx1);
        }
        if (daggeratkfx2 != null)
        {
            fxDic.Add(daggeratkfx2.name, daggeratkfx2);
        }
        if (daggeratkfx3 != null)
        {
            fxDic.Add(daggeratkfx3.name, daggeratkfx3);
        }
        if (daggeratkfx4 != null)
        {
            fxDic.Add(daggeratkfx4.name, daggeratkfx4);
        }
        if (daggeratkfx5 != null)
        {
            fxDic.Add(daggeratkfx5.name, daggeratkfx5);
        }


        camTrans = Camera.main.transform;
        offSet = transform.position - camTrans.position;
        currentBlend = 0;
    }
    private void Update()
    {
        if (currentBlend != targetBlend)
        {
            AnimLerpFunc();
        }

        if (isMove)
        {
            SetDir();
            SetMove();
            SetCam();
        }

        if(skillMove == true)
        {
            SetSkillMove();
            SetCam();
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

    private void SetSkillMove()
    {
        pContr.Move(transform.forward * Time.deltaTime * skillMoveSpeed);
    }

   /// <summary>
   /// 设置摄像机跟随
   /// </summary>
    public void SetCam()
    {
        if(camTrans != null)
        {
            camTrans.position = transform.position - offSet;
        }
    }

    /// <summary>
    /// 动画转换
    /// </summary>
    /// <param name="blend"></param>
    //public void AnimTrans()
    //{       
    //    animC.SetFloat("Blend", currentBlend);
    //}

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
        if(Mathf.Abs(currentBlend - targetBlend) < Constants.transSpeed * Time.deltaTime)
        {
            currentBlend = targetBlend;
            //Debug.Log(currentBlend + "   " + targetBlend);
        }
        else if(currentBlend < targetBlend)
        {
            currentBlend += Constants.transSpeed * Time.deltaTime;
        }
        else
        {
            currentBlend -= Constants.transSpeed * Time.deltaTime;
        }
        animC.SetFloat("Blend", currentBlend);
    }

    public override void SetFx(string name, float destroyTime)
    {
        GameObject go;
        if(fxDic.TryGetValue(name, out go))
        {
            go.SetActive(true);
            timerSvc.AddTimeTask((tid) =>
            {
                go.SetActive(false);
            },destroyTime);
        }
    }

   
}