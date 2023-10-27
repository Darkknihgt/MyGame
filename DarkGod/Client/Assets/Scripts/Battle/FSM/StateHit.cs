/****************************************************
    文件：StateHit.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/8/2 14:59:20
	功能：被攻击状态
*****************************************************/

using UnityEngine;

public class StateHit : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentAniState = AniState.Hit;
        entity.RmvSkillCB();
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        
    }

    public void Process(EntityBase entity, params object[] args)
    {
        if (entity.entityType == EntityType.Player)
        {
            entity.canRlsSkill = false;
        }

        entity.SetDir(Vector2.zero);
        entity.SetAction(Constants.ActionHit);

        //受击音效
        if(entity.entityType == EntityType.Player)
        {
            AudioSource charAudio = entity.GetAudio();
            AudioSvc.Instance.PlayCharMusic(Constants.AssassinHit,charAudio);
        }

        TimerSvc.Instance.AddTimeTask((int tid)=> {
            entity.SetAction(Constants.ActionDefault);
            entity.Idle();
        },(int)(GetHitAniLen(entity) * 1000));
    }

    private float GetHitAniLen(EntityBase entity)
    {
        AnimationClip[] clips = entity.GetEntityCtrl().animC.runtimeAnimatorController.animationClips;
        for(int i = 0; i < clips.Length; i++)
        {
            string clipName = clips[i].name;
            if(clipName.Contains("Hit") ||
                clipName.Contains("hit") ||
                clipName.Contains("HIT"))
            {
                return clips[i].length;
            }
        }
        return 1;
    }
}