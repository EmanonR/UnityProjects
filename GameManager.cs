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
    public bool playerFrozen;
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

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Application.targetFrameRate = PlayerPrefs.GetInt("TargetFPS");

        InstantiatePlayer();
    }

    public void InstantiatePlayer()
    {
        GameObject newPlayer = Instantiate(playerPrefab);
        newPlayer.transform.position = saveData.mapPosition;
        newPlayer.transform.eulerAngles = saveData.orientation;
        newPlayer.GetComponent<PlayerController>().frozen = true;
        playerFrozen = true;

        player = newPlayer.transform;

        DontDestroyOnLoad(newPlayer);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gamePaused = !gamePaused;
        }

        Time.timeScale = gamePaused ? 0f : 1f;

        player.GetComponent<PlayerController>().frozen = playerFrozen || gamePaused;

        if (pausePanel != null)
            pausePanel.SetActive(gamePaused);

    }

    public void UpdateFPS(string value)
    {
        SaveSystem.SavePrefInt("TargetFPS", int.Parse(value));
        Application.targetFrameRate = int.Parse(value);
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
