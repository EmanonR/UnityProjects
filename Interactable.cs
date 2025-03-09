using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float InteractRange = 5f;
    public Vector3 InteractOffset;

    private void Update()
    {
        Transform player = GameManager.instance.player.transform;
        if (Vector3.Distance(player.position, transform.position + InteractOffset) <= InteractRange)
        {
            if (Input.GetKeyDown(GameManager.instance.confirm))
            {
                Interact();
            }
        }
    }

    public virtual void Interact()
    {
        print("Interacting...");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + InteractOffset, InteractRange);
    }
}
