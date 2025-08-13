using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private float somVolume = 0.5f;
    [SerializeField] private float narrativaVolume = 1.0f;
    [SerializeField] private float musicaVolume = 0.3f;

    private static SoundPlayer Instance;
    private AudioSource bgMusic;
    private AudioSource soundFX;
    private AudioSource voice;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        //Verifica se tem PlayerPrefs salvos
        if (PlayerPrefs.HasKey("SomVolume"))
        {
            somVolume = PlayerPrefs.GetFloat("SomVolume");
        }
        else
        {
            SaveVolumes();
        }

        if (PlayerPrefs.HasKey("MusicaVolume"))
        {
            musicaVolume = PlayerPrefs.GetFloat("MusicaVolume");
        }
        else
        {
            SaveVolumes();
        }

        if (PlayerPrefs.HasKey("NarrativaVolume"))
        {
            narrativaVolume = PlayerPrefs.GetFloat("NarrativaVolume");
        }
        else
        {
            SaveVolumes();
        }



        bgMusic = GameObject.Find("BgMusic").GetComponent<AudioSource>();
        soundFX = GameObject.Find("SoundFX").GetComponent<AudioSource>();
        voice = GameObject.Find("Voice").GetComponent<AudioSource>();

        DontDestroyOnLoad(gameObject);

        SetVolumes();
    }

    public void PlaySoundBackground(AudioClip clip)
    {
        if(clip.name != bgMusic.clip.name)
        {
            SetVolumes();
            bgMusic.clip = clip;
            bgMusic.Play();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        SetVolumes();
        soundFX.PlayOneShot(clip);
    }

    public void PlayNarrativa(AudioClip clip)
    {
        SetVolumes();
        voice.clip = clip;
        voice.Play();
    }

    public void SetSomVolume(float volume)
    {
        somVolume = volume;
        soundFX.volume = volume;
    }

    public void SetMusicaVolume(float volume)
    {
        musicaVolume = volume;
        bgMusic.volume = volume;
    }

    public void SetNarrativaVolume(float volume)
    {
        narrativaVolume = volume;
        voice.volume = volume;
    }

    public float GetSomVolume()
    {
        return somVolume;
    }

    public float GetMusicaVolume()
    {
        return musicaVolume;
    }

    public float GetNarrativaVolume()
    {
        return narrativaVolume;
    }

    private void SetVolumes()
    {
        bgMusic.volume = musicaVolume;
        soundFX.volume = somVolume;
        voice.volume = narrativaVolume;
    }

    public void SaveVolumes()
    {
        PlayerPrefs.SetFloat("SomVolume", somVolume);
        PlayerPrefs.SetFloat("MusicaVolume", musicaVolume);
        PlayerPrefs.SetFloat("NarrativaVolume", narrativaVolume);
        PlayerPrefs.Save();
    }

    private void OnLevelWasLoaded(int level)
    {
        voice.Stop();
    }
}
