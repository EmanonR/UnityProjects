using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : Interactable
{
    public Dialogue dialogueData;

    public override void Interact()
    {
        if (DialogueManager.instance.inDialogue) return;

        if (dialogueData == null)
        {
            print("There is no dialogueData, attach one!");
            return;
        }

        DialogueManager.instance.StartDialogue(dialogueData);
    }
}
