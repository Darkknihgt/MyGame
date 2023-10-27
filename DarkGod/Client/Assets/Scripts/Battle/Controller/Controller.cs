/****************************************************
    文件：Controller.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/20 23:24:34
	功能：行为控制基类
*****************************************************/
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller :MonoBehaviour
{
    protected Transform camTrans;

    public Animator animC;
    public CharacterController pContr;

    public Transform HpRoot;

    protected bool isMove;
    public bool canControl = true;
    private Vector2 dir;
    public Vector2 Dir
    {
        get
        {
            return dir;
        }

        set
        {
            if (value == Vector2.zero)
            {
                isMove = false;
            }
            else
            {
                isMove = true;
            }
            dir = value;
        }
    }

    //战斗移动
    protected bool skillMove = false;
    protected float skillMoveSpeed = 0f;

    protected Dictionary<string, GameObject> fxDic = new Dictionary<string, GameObject>();
    protected TimerSvc timerSvc = null;

    public virtual void Init()
    {
        timerSvc = TimerSvc.Instance;
    }

    public virtual void SetBlend(float blend)
    {
        animC.SetFloat("Blend",blend);
    }

    public virtual void SetAction(int act)
    {
        animC.SetInteger("Action", act);
    }

    /// <summary>
    /// 启动特效
    /// </summary>
    /// <param name="特效名字"></param>
    /// <param 特效持续时间="destroyTime"></param>
    public virtual void SetFx(string name,float destroyTime)
    {
        
    }

    public void SetSkillMoveState(bool skillMoveState,float skillSpeed)
    {
        skillMove = skillMoveState;
        skillMoveSpeed = skillSpeed;
    }

    public virtual void SetAtkRotationLocal(Vector2 atkDir)
    {
        float angle = Vector2.SignedAngle(atkDir, new Vector2(0, 1));
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        pContr.transform.eulerAngles = eulerAngle;
    }

    public virtual void SetAtkRotation(Vector2 atkDir)
    {
        float angle = Vector2.SignedAngle(atkDir, new Vector2(0, 1)) + camTrans.eulerAngles.y;
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        pContr.transform.eulerAngles = eulerAngle;  
    }
}

