/****************************************************
    文件：MonsterController.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/7/30 16:22:14
	功能：怪物表现类
*****************************************************/

using UnityEngine;

public class MonsterController : Controller
{
    private void Update()
    {
        if (isMove)
        {
            SetDir();
            SetMove();
        }
    }

    private void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1));
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }

    private void SetMove()
    {
        pContr.Move(transform.forward * Time.deltaTime * Constants.EnemySpeed);

        //给一个向下的速度，便于没有apply root时使得怪物浮空
        pContr.Move(Vector3.down * Time.deltaTime * Constants.EnemySpeed);
    }
}