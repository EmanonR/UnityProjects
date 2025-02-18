using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<ItemSlotClass> normalItems;
    public List<ItemSlotClass> keyItems;
    public int money;

    public static event Action<Item> ItemAdded;
    public static event Action<Item> ItemRemoved;

    public void AddItem(Item itemToAdd)
    {
        switch (itemToAdd.itemType)
        {
            case Item.ItemType.normal:
                AddToList(normalItems, itemToAdd);
                break;

            case Item.ItemType.keyItem:
                AddToList(keyItems, itemToAdd);
                break;

            case Item.ItemType.money:
                money += itemToAdd.value;
                break;

            default:
                break;
        }

        ItemAdded?.Invoke(itemToAdd);
    }

    public void RemoveItem(Item itemToRemove)
    {
        switch (itemToRemove.itemType)
        {
            case Item.ItemType.normal:
                RemoveFromList(normalItems, itemToRemove);
                break;

            case Item.ItemType.keyItem:
                RemoveFromList(keyItems, itemToRemove);
                break;

            case Item.ItemType.money:
                money -= itemToRemove.value;
                break;

            default:
                break;
        }

        ItemRemoved?.Invoke(itemToRemove);
    }


    void AddToList(List<ItemSlotClass> listToAdd, Item itemToAdd)
    {
        //New item slot if no slots or item is not stackable
        if (listToAdd.Count == 0 || !itemToAdd.stackable)
        {
            //Make new itemslot and add item to it with a count of 1
            listToAdd.Add(new ItemSlotClass()
            {
                amount = 1,
                itemInList = itemToAdd
            });
            return;
        }

        //Check for itemslot with item
        for (int i = 0; i < listToAdd.Count; i++)
        {
            //If found, increase amount in respective itemslot
            if (listToAdd[i].itemInList == itemToAdd)
            {
                listToAdd[i].amount++;
                return;
            }
        }

        // If item slot doesnt exist
        listToAdd.Add(new ItemSlotClass()
        {
            amount = 1,
            itemInList = itemToAdd
        });
        return;
    }

    void RemoveFromList(List<ItemSlotClass> listToRemove, Item itemToRemove)
    {
        //Check for itemslot with item
        for (int i = 0; i < listToRemove.Count; i++)
        {
            //If found
            if (listToRemove[i].itemInList == itemToRemove)
            {
                if (listToRemove[i].amount == 1)
                {
                    //Remove itemslot
                    listToRemove.RemoveAt(i);
                }
                else
                {
                    //decrease amount in respective itemslot
                    listToRemove[i].amount--;
                }
            }
        }
    }    
}

[System.Serializable]
public class ItemSlotClass
{
    public Item itemInList;
    public int amount;
}
