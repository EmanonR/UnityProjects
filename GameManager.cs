using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public Transform player;

    [Header("setup")]
    public GameObject playerPrefab;
    public Vector3 playerSpawnLocation;
    [SerializeField] int targetFPS;

    [Header("Pausing")]
    public bool gamePaused;
    [SerializeField] GameObject pausePanel;

    [Header("Audio")]
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    [Range(-80,0)]
    [SerializeField] float masterVolume, musicVolume, sfxVolume;

    [Header("Global keyCodes")]
    public KeyCode confirm = KeyCode.Z;
    public KeyCode cancel = KeyCode.X;
    public KeyCode hideUI = KeyCode.A;
    public KeyCode skipText = KeyCode.LeftControl;


    public static GameManager instance;

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
        Application.targetFrameRate = targetFPS;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        GetComponents();
    }

    void GetComponents()
    {
        // Ensure player is assigned when scene changes
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
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
        musicSource.clip = newMusic;
    }

    public void PlaySFX(AudioClip audio)
    {
        sfxSource.clip = audio;
        sfxSource.Play();
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
        playerSpawnLocation = newLocation;

        // Ensure the player is moved after the next scene load
        if (player == null)
        {
            GetComponents();
        }

        player.position = newLocation;

    }
}
