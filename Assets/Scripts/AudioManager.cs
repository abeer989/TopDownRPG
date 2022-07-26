using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] List<AudioSource> music;
    [SerializeField] List<AudioSource> SFX;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        else
        {
            if (instance != this)
                Destroy(gameObject);
        }
    }

    public void PlaySFX(int sfxIndex, bool adjust = false)
    {
        if (sfxIndex < SFX.Count)
        {
            if (adjust)
                SFX[sfxIndex].pitch = Random.Range(1, 1.2f);

            SFX[sfxIndex].Play(); 
        }
    }

    public void PlayMusic(int musicIndex)
    {
        if (musicIndex < music.Count)
        {
            if (!music[musicIndex].isPlaying)
            {
                music.ForEach(a => a.Stop());
                music[musicIndex].Play();  
            }
        }
    }
}
