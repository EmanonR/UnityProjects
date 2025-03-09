using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum DialogueState { printing, lineEnd, choice, end }

public class DialogueManager : MonoBehaviour
{
    #region Variables
    [Header("UI")]
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] GameObject dialogueBox, dialogueNamebox;
    TMP_Text dialogueText, dialogueNameText;

    [Header("Debug")]
    [SerializeField] int cps = 40;
    public bool inDialogue = false;

    int dialogueIndex, lineIndex, charIndex;
    Dialogue currentDialogueObject;
    DialogueLine currentDialogue;
    string currentLine;
    string printedLine;

    float charTimer;
    bool canSkip;
    DialogueState state;
    public static DialogueManager instance;
    #endregion

    private void Awake()
    {
        dialoguePanel.SetActive(false);

        if (instance != null) Destroy(this);
        instance = this;

        dialogueNameText = dialogueNamebox.GetComponentInChildren<TMP_Text>();
        dialogueText = dialogueBox.GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        if (!inDialogue) return;

        switch (state)
        {
            case DialogueState.printing:
                //Update Line
                UpdateLine();

                if (Input.GetKeyDown(GameManager.instance.confirm) || Input.GetKey(GameManager.instance.skipText))
                    if (canSkip)
                        charIndex = currentLine.Length -1;

                //Update UI
                UpdateUI();

                if (charIndex == currentLine.Length)
                    state = DialogueState.lineEnd;
                break;

            case DialogueState.lineEnd:
                //Await Input
                if (Input.GetKeyDown(GameManager.instance.confirm) || Input.GetKey(GameManager.instance.skipText))
                    if (canSkip)
                        NewLine();
                break;

            case DialogueState.choice:
                //Display Choices
                //TBA
                break;

            case DialogueState.end:
                //End Dialogue
                EndDialogue();
                break;
        }

        canSkip = true;
    }

    public void StartDialogue(Dialogue newDialogue)
    {
        dialoguePanel.SetActive(true);

        canSkip = false;
        inDialogue = true;
        currentDialogueObject = newDialogue;
        dialogueIndex = 0; 
        lineIndex = -1;
        charIndex = 0;

        currentDialogue = currentDialogueObject.dialogues[dialogueIndex];
        NewLine();
    }

    public void EndDialogue()
    {
        dialogueText.text = "";
        dialogueNameText.text = "";
        currentLine = "";
        printedLine = "";

        dialoguePanel.SetActive(false);
        inDialogue = false;
        currentDialogue = null;
    }

    void NewLine()
    {
        // Advance Line
        lineIndex++;
        charIndex = 0;

        // If at end of Lines
        if (lineIndex >= currentDialogue.lines.Count)
        {
            // Advance Diaogue
            dialogueIndex++;


            // If at end of Dialogues
            if (dialogueIndex >= currentDialogueObject.dialogues.Count)
            {
                // End Dialogue
                state = DialogueState.end;
                return;
            }

            currentDialogue = currentDialogueObject.dialogues[dialogueIndex];

            // Reset LineIndex
            lineIndex = 1;
        }

        currentLine = currentDialogue.lines[lineIndex];

        if (currentLine == "") currentLine = "...";
        // Print New line
        state = DialogueState.printing;
    }

    void UpdateLine()
    {
        if (cps == 0)
        {
            printedLine = currentLine;
            return;
        }

        charTimer += Time.deltaTime;

        if (charTimer >= 1f / cps)
        {
            charTimer = 0;
            charIndex++;
            printedLine = currentLine.Substring(0, charIndex);
            if (currentDialogue.speakSound != null)
                //Play Sound
                ;
        }
    }

    void UpdateUI()
    {
        dialogueNameText.text = currentDialogue.name;
        dialogueText.text = printedLine;
    }
}
