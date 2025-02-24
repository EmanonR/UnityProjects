using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int targetFPS;

    public Transform player;
    public InventoryManager inventoryManager;

    public bool gamePaused;
    public GameObject pausePanel;

    public static GameManager instance;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        Application.targetFrameRate = targetFPS;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        inventoryManager = GetComponent<InventoryManager>();

        if (pausePanel != null)
            pausePanel.SetActive(gamePaused);
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
}
