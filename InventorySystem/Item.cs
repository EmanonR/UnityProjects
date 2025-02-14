using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Custom/Item")]
public class Item : ScriptableObject
{
    public Sprite icon;
    public new string name;
    public bool stackable;
    public int value;

    public ItemType itemType;
    public OnUse onUse;

    public enum ItemType
    {
        normal,
        keyItem,
        money
    }

    public enum OnUse
    {
        none,
        consumable,
        equipable
    }
}
