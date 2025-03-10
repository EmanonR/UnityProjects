using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    [Header("Battle Data")]
    public BattleLayout currentBattleLayout;
    public List<GameObject> playerParty;
    
    [Header("Scene transition")]
    public int battleSceneIndex;
    public Vector3 battleTriggerLocation;
    public int previousScene;
    public GameObject enemyBattler;
    
    public static CombatManager instance;
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
        
        SceneManager.LoadSceneAsync(battleSceneIndex);
    }
    
    public void EndBattle()
    {
        int previousSceneIndex = previousScene;
        currentBattleLayout = null;
        
        SceneManager.LoadSceneAsync(previousSceneIndex);
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == battleSceneIndex && currentBattleLayout != null)
        {
            InitializeBattleScene();
        }
    }
    
    private void InitializeBattleScene()
    {
        CombatSceneController combatSceneController = FindObjectOfType<CombatSceneController>();
        if (combatSceneController != null)
        {
            combatSceneController.SetupBattle(currentBattleLayout);
        }
        else
        {
            Debug.LogError("CombatSceneController not found in battle scene!");
        }
    }
}
