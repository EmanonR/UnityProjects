using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [Header("Battle Configuration")]
    public Transform[] enemySpawnPoints;
    public GameObject[] spawnedEnemies;
    
    [Header("Battle State")]
    public bool battleInProgress = false;
    public int currentTurn = 0;
    
    private BattleLayout currentBattle;
    
    public void SetupBattle(BattleLayout battleLayout)
    {
        currentBattle = battleLayout;
        SpawnEnemies();
        StartBattle();
    }
    
    private void SpawnEnemies()
    {
        // Clear any existing enemies
        if (spawnedEnemies != null)
        {
            foreach (GameObject enemy in spawnedEnemies)
            {
                if (enemy != null)
                    Destroy(enemy);
            }
        }
        
        // Initialize enemies array
        spawnedEnemies = new GameObject[currentBattle.enemies.Length];
        
        // Spawn each enemy at designated points
        for (int i = 0; i < currentBattle.enemies.Length; i++)
        {
            if (i < enemySpawnPoints.Length)
            {
                GameObject enemyObject = Instantiate(
                    currentBattle.enemies[i].enemyPrefab,
                    enemySpawnPoints[i].position,
                    enemySpawnPoints[i].rotation
                );
                
                // Set up enemy stats
                EnemyBattleEntity enemyEntity = enemyObject.GetComponent<EnemyBattleEntity>();
                if (enemyEntity != null)
                {
                    enemyEntity.Initialize(currentBattle.enemies[i]);
                }
                
                spawnedEnemies[i] = enemyObject;
            }
            else
            {
                Debug.LogWarning("Not enough spawn points for all enemies!");
                break;
            }
        }
    }
    
    private void StartBattle()
    {
        battleInProgress = true;
        currentTurn = 0;
        
        // Initialize first turn
        StartNextTurn();
    }
    
    private void StartNextTurn()
    {
        currentTurn++;
        // Implement turn logic here
    }
    
    public void EndBattle(bool playerVictory)
    {
        battleInProgress = false;
        
        if (playerVictory)
        {
            // Handle rewards, experience, etc.
        }
        
        // Return to previous scene
        CombatManager.instance.EndBattle();
    }
}
