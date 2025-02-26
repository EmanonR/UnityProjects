using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Battle Layout", menuName = "Custom/Battle")]
public class BattleLayout : ScriptableObject
{
    public EnemyType[] enemies;
}
