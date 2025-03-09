using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Custom/Dialogue")]
public class Dialogue : ScriptableObject
{
    public List<DialogueLine> dialogues;
}

[System.Serializable]
public class DialogueLine
{
    public string name = "NoName";
    [TextArea(3,10)]
    public List<string> lines;
    public List<string> choices;

    [Header("Quality of life")]
    public Image charSprite;
    public AudioClip speakSound;
}
