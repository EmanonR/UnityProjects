using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Enemy", menuName = "Custom/Enemy")]
public class EnemyType : ScriptableObject
{
    public new string name = "Default";
    public string bio = "No description";
    public int hp = 75, dmg = 5, def = 10, spd = 1;
    public GameObject enemyPrefab;

    public Attack[] attacks;
}

[System.Serializable]
public class Attack
{
    public string name = "Default Attack";
    public string description = "A basic attack";
    public int baseDamage = 10;

    public enum AttackType
    {
        Physical,
        Magic,
        Status
    }

    public enum TargetType
    {
        SingleEnemy,
        AllEnemies,
        Self,
        SingleAlly,
        AllAllies
    }

    public AttackType attackType = AttackType.Physical;
    public TargetType targetType = TargetType.SingleEnemy;

    [Header("Effects")]
    public bool causesStatusEffect = false;
    public StatusEffect statusEffect;

    [System.Serializable]
    public class StatusEffect
    {
        public string effectName;
        public int duration;
        public float effectValue;
    }
}
