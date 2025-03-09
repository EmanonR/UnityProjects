using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : Interactable
{
    public BattleLayout battle;

    public override void Interact()
    {
        CombatManager.instance.StartBattle(battle, gameObject);
    }
}
