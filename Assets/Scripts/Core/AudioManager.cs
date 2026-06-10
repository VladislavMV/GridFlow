using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer Settings")]
    [SerializeField] private AudioMixer mainMixer;

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Lists")]
    public Sound[] sfxSounds;
    public Sound musicTrack;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (musicTrack.clip != null)
        {
            PlayMusic(musicTrack.clip);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null) return;

        sfxSource.PlayOneShot(s.clip);
    }

    public void PlayWeaponSFX(AudioClip clip)
    {
        if (clip == null) return;

        sfxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float value)
    {
        mainMixer.SetFloat("MusicVol", Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float value)
    {
        mainMixer.SetFloat("SFXVol", Mathf.Log10(value) * 20);
    }
}

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}