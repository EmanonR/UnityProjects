using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    [Header("Battle Data")]
    public BattleLayout currentBattleLayout;
    public List<GameObject> playerParty;
    private bool returningFromBattle = false;
    
    [Header("Scene transition")]
    public int battleSceneIndex;
    public Vector3 battleTriggerLocation;
    public int previousScene;
    public GameObject enemyBattler;
    
    public static CombatManager instance;

    public Dictionary<GameObject, bool> enemyStatus = new Dictionary<GameObject, bool>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    public void StartBattle(BattleLayout battle, GameObject enemy)
    {
        //Save current game state
        enemyBattler = enemy;
        battleTriggerLocation = GameManager.instance.player.position;
        currentBattleLayout = battle;
        previousScene = SceneManager.GetActiveScene().buildIndex;
        
        SceneManager.LoadScene(battleSceneIndex);
    }
    
    public void EndBattle()
    {
        returningFromBattle = true;
        int previousSceneIndex = previousScene;
        currentBattleLayout = null;
        
        SceneManager.LoadScene(previousSceneIndex);
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == battleSceneIndex && currentBattleLayout != null)
        {
            InitializeBattleScene();
        }
        
        if (returningFromBattle && scene.buildIndex == previousScene)
        {
            StartCoroutine(RestorePlayerPositionAfterDelay());
            returningFromBattle = false;
        }
    }
    
    private void InitializeBattleScene()
    {
        BattleController battleController = FindObjectOfType<BattleController>();
        if (battleController != null)
        {
            battleController.SetupBattle(currentBattleLayout);
        }
        else
        {
            Debug.LogError("BattleController not found in battle scene!");
        }
    }
    
    private IEnumerator RestorePlayerPositionAfterDelay()
    {
        yield return null;
        
        if (GameManager.instance != null)
        {
            GameManager.instance.SetPlayerLocation(battleTriggerLocation);
        }
    }


    public void RegisterEnemy(GameObject enemy)
    {
        if (!enemyStatus.ContainsKey(enemy))
            enemyStatus.Add(enemy, false); // False means "not defeated"
    }

    public void MarkDefeated(GameObject enemy)
    {
        if (enemyStatus.ContainsKey(enemy))
            enemyStatus[enemy] = true;
    }

    public bool IsDefeated(GameObject enemy)
    {
        return enemyStatus.TryGetValue(enemy, out bool defeated) && defeated;
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemyStatus.Remove(enemy);
    }

}
