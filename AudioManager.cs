using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public int AudioSources = 2;

    [Header("Audio")]
    [SerializeField] AudioMixer mixer;
    [Range(-80, 0)]
    [SerializeField] float masterVolume, musicVolume, sfxVolume;

    [HideInInspector] public AudioSource[] audioSources;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        audioSources = GetComponentsInChildren<AudioSource>();
        int missingSources = AudioSources - audioSources.Length;

        if (missingSources > 0)
        {
            for (int i = 0; i < missingSources; i++)
            {
                gameObject.AddComponent<AudioSource>();
            }

            audioSources = GetComponentsInChildren<AudioSource>();
        }

        audioSources[0].loop = true;
        audioSources[1].loop = true;
    }

    private void Update()
    {
        UpdateVolumeSettings();
    }

    public void ChangeMusic(AudioClip newMusic)
    {
        audioSources[0].clip = newMusic;
        audioSources[0].Play();
    }

    public void PlaySFX(AudioClip audio)
    {
        audioSources[1].clip = audio;
        audioSources[1].Play();
    }

    void UpdateVolumeSettings()
    {
        mixer.SetFloat("MasterVolume", masterVolume);
        mixer.SetFloat("MusicVolume", musicVolume);
        mixer.SetFloat("SFXVolume", sfxVolume);
    }
}
