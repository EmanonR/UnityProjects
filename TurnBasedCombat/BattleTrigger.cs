using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    public float triggerRange = 1f;
    public BattleLayout battle;

    Transform player;
    bool triggered;

    private void Start()
    {
        player = GameManager.instance.player;
    }


    public void Update()
    {
        if (player == null) player = GameManager.instance.player;
        if (triggered || player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= triggerRange)
        {
            print("Start battle!");
            //Trigger Battle
            Destroy(gameObject);

            CombatManager.instance.StartBattle(battle);
            triggered = true;

        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, triggerRange);
    }
}
