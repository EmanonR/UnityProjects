using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform player;

    [Header("setup")]
    public GameObject playerPrefab;
    public Vector3 playerSpawnLocation;
    [SerializeField] int targetFPS;

    [Header("Pausing")]
    public bool gamePaused;
    public GameObject pausePanel;


    public static GameManager instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = targetFPS;

        if (pausePanel != null)
            pausePanel.SetActive(gamePaused);

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
    }

    public void PauseSwitch()
    {
        gamePaused = !gamePaused;
    }

    public void SetPlayerLocation(Vector3 newLocation)
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
