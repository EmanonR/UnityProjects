using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
  public ItemSlot[] itemSlots;
}

public class ItemSlot 
{
  public List<Item> itemsInSlot = new;
}
