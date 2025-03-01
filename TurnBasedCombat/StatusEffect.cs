using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unnamed Effect", menuName = "Custom/Effect")]
public class StatusEffect : ScriptableObject
{
    public int duration;
    public float effectValue;
}
