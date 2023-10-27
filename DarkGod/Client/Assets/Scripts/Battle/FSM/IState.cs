/****************************************************
    文件：IState.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/19 12:58:7
	功能：状态接口
*****************************************************/

public interface IState
{
    void Enter(EntityBase entity,params object[] args);
    void Process(EntityBase entity, params object[] args);
    void Exit(EntityBase entity, params object[] args);
}
public enum AniState
{
    None,
    Born,
    Idle,
    Move,
    Attack,
    Hit,
    Die,
}
