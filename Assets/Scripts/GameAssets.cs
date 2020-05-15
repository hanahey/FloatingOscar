using System;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;

    public static GameAssets GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }
    
    public Transform pfClouds_1;
    public Transform pfClouds_2;
    public Transform pfClouds_3;
    public Transform pfBranch;
    public Transform pfTreeBodyRight;
    public Transform pfTreeBodyLeft;

    public SoundAudioClip[] soundAudioClips;

    [Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
}
