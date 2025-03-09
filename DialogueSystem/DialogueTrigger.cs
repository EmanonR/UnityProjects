using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public float InteractRange = 5f;
    public Vector3 InteractOffset;

    public Dialogue dialogueData;

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

    void Interact()
    {
        if (DialogueManager.instance.inDialogue) return;

        if (dialogueData == null)
        {
            print("There is no dialogueData, attach one!");
            return;
        }

        DialogueManager.instance.StartDialogue(dialogueData);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + InteractOffset, InteractRange);
    }
}
