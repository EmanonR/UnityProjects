using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    #region variables
    [Header("Setup")]
    public GameObject playerPrefab;

    [Header("Pausing")]
    public bool gamePaused;
    [SerializeField] GameObject pausePanel;

    [Header("Audio")]
    [SerializeField] AudioMixer mixer;
    [Range(-80,0)]
    [SerializeField] float masterVolume, musicVolume, sfxVolume;

    [Header("Global keyCodes")]
    public KeyCode confirm = KeyCode.Z;
    public KeyCode cancel = KeyCode.X;
    public KeyCode hideUI = KeyCode.A;
    public KeyCode skipText = KeyCode.LeftControl;

    public static GameManager instance;
    [HideInInspector] public SaveData saveData;

    [HideInInspector] public AudioSource[] audioSources;
    [HideInInspector] public Transform player;

    #endregion

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

        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = PlayerPrefs.GetInt("TargetFPS");

        if (pausePanel != null)
            pausePanel.SetActive(false);

        audioSources = GetComponentsInChildren<AudioSource>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gamePaused = !gamePaused;
        }

        Time.timeScale = gamePaused ? 0f : 1f;

        if (pausePanel != null)
            pausePanel.SetActive(gamePaused);

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

    public void PauseSwitch()
    {
        gamePaused = !gamePaused;
    }

    public void SetPlayerPosition(Vector3 newLocation)
    {
        player.position = newLocation;
    }
}
