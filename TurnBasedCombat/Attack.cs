using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unnamed Attack", menuName = "Custom/Attack")]
public class Attack : ScriptableObject
{
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
}
