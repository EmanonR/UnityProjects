using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
  public sprite icon;
  public new string name;

  public bool isKeyItem, stackable;

  public enum onUse {
    none,
    consumable,
    equipable
  }

  void useItem()
  {
  }
}
