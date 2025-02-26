using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattleEntity : MonoBehaviour
{
    [Header("Stats")]
    public string enemyName;
    public int maxHp, currentHp, damage, defense, speed;
    
    [Header("Attack Options")]
    public EnemyType baseEnemyType;
    public Attack[] availableAttacks;
    
    public void Initialize(EnemyType enemyType)
    {
        baseEnemyType = enemyType;
        
        // Set stats from enemyType
        enemyName = enemyType.name;
        maxHp = enemyType.hp;
        currentHp = maxHp;
        damage = enemyType.dmg;
        defense = enemyType.def;
        speed = enemyType.spd;
        availableAttacks = enemyType.attacks;
        
        // Additional initialization if needed
    }
    
    public void TakeDamage(int amount)
    {
        int actualDamage = Mathf.Max(1, amount - defense);
        currentHp -= actualDamage;
        
        if (currentHp <= 0)
        {
            Die();
        }
    }
    
    public Attack ChooseAttack()
    {
        // Simple AI for now - just pick a random attack
        if (availableAttacks != null && availableAttacks.Length > 0)
        {
            int attackIndex = Random.Range(0, availableAttacks.Length);
            return availableAttacks[attackIndex];
        }
        
        return null;
    }
    
    private void Die()
    {
        // Handle enemy death
        BattleController battleController = FindObjectOfType<BattleController>();
        
        // Check if all enemies are defeated
        if (battleController != null)
        {
            bool allDefeated = true;
            foreach (GameObject enemy in battleController.spawnedEnemies)
            {
                if (enemy != null && enemy != this.gameObject)
                {
                    EnemyBattleEntity otherEnemy = enemy.GetComponent<EnemyBattleEntity>();
                    if (otherEnemy != null && otherEnemy.currentHp > 0)
                    {
                        allDefeated = false;
                        break;
                    }
                }
            }
            
            if (allDefeated)
            {
                battleController.EndBattle(true);
            }
        }
        
        // Disable or destroy this enemy
        gameObject.SetActive(false);
    }
}
