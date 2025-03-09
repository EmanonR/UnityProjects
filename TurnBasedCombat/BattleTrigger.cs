using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : Interactable
{
    public BattleLayout battle;

    public override void Interact()
    {
        if (battle == null)
        {
            print("There is no battleLayout attached, attach one!");
            return;
        }

        CombatManager.instance.StartBattle(battle, gameObject);
    }
}
