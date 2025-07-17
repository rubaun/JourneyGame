using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    private AudioSource player;
    [SerializeField] private float somVolume = 0.5f;
    [SerializeField] private float musicaVolume = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);

        //Find other AudioSources in the scene and destroy them
        GameObject[] audioSources = GameObject.FindGameObjectsWithTag("Audio");
        foreach (GameObject audioSource in audioSources)
        {
            if (audioSource == this.gameObject && audioSources.Length > 1)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void PlaySoundBackground(AudioClip clip)
    {
        if (clip != null && player != null && player.isPlaying && player.clip.name != clip.name)
        {
            player.clip = clip;
            player.volume = musicaVolume;
            player.Play();
        }
        else if(clip != null && player != null && !player.isPlaying)
        {
            player.clip = clip;
            player.volume = musicaVolume;
            player.Play();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && player != null)
        {
            player.volume = somVolume;
            player.PlayOneShot(clip);
        }
    }

    public void SetSomVolume(float volume)
    {
        somVolume = volume;
    }

    public void SetMusicaVolume(float volume)
    {
        musicaVolume = volume;
        if (player.isPlaying)
        {
            player.volume = musicaVolume;
        }
    }

    public float GetSomVolume()
    {
        return somVolume;
    }

    public float GetMusicaVolume()
    {
        return musicaVolume;
    }
}
