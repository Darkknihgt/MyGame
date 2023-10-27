/****************************************************
    文件：AudioSvc.cs
	作者：YinQiXuan
    邮箱: 864061033@qq.com
    日期：2023/5/12 18:2:59
	功能：声音处理系统
*****************************************************/

using UnityEngine;

public class AudioSvc : MonoBehaviour 
{
    public static AudioSvc Instance = null;

    public AudioSource BGAudio;
    public AudioSource UIAudio;

    public void InitAudioSvc()
    {
        Instance = this;

        PECommon.Log("Init AudioSvc...");
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isLoop"></param>
    public void PlayBGMusic(string name,bool isLoop = true)
    {
        //资源加载系统获得加载的音乐资源，并用临时变量存储
        AudioClip audio = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);

        if(BGAudio.clip ==null || BGAudio.name != audio.name)
        {
            BGAudio.clip = audio;
            BGAudio.loop = isLoop;
            BGAudio.Play();
        }
    }

    public void PlayUIMusic(string name)
    {
        AudioClip audio = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);

        UIAudio.clip = audio;
        UIAudio.loop = false;
        UIAudio.Play();
    }

    public void PlayCharMusic(string name,AudioSource audioSource)
    {
        AudioClip audio = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);

        audioSource.clip = audio;
        audioSource.loop = false;
        audioSource.Play();
    }

    public void StopBGMusic()
    {
        if(BGAudio != null)
        {
            BGAudio.Stop();
        }
    }
}