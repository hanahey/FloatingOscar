using UnityEngine;

public static class SoundManager
{ 
    public enum Sound
    {
        Bark, //sound played when scoring
        Whine, //sound played when branch is hit
        ButtonClick,
        ButtonOver,
    }

    public static void PlaySound(Sound sound)
    {
        GameObject gameObject = new GameObject("Sound", typeof(AudioSource));
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(GetAudioClip(sound));
    }

    public static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.GetInstance().soundAudioClips)
        {
            if (soundAudioClip.sound == sound)
               return soundAudioClip.audioClip;
        }
        Debug.LogError("Sound" + sound + " not found!");
        return null;
    }
}
