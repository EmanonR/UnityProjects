using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region variables
    [Header("Setup")]
    public GameObject playerPrefab;

    [Header("Pausing")]
    public bool gamePaused;
    [SerializeField] GameObject pausePanel;

    [Header("Global keyCodes")]
    public KeyCode confirm = KeyCode.Z;
    public KeyCode cancel = KeyCode.X;
    public KeyCode hideUI = KeyCode.A;
    public KeyCode skipText = KeyCode.LeftControl;

    public static GameManager instance;
    [HideInInspector] public SaveData saveData;

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
