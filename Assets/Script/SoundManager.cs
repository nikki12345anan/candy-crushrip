using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;

    public static SoundManager instance;
    public AudioSource[] DestroyNoise;

    void Awake()
    {
        
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        //sesi oyuna ekle
        foreach(Sound s in sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.clip;
            s.Source.volume = s.volume;
            s.Source.pitch = s.pitch;
            s.Source.loop = s.loop;
        }
    }

    

    public void play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning(name + "bulunamadi");
            return;
        }
        s.Source.Play();
    }
}
