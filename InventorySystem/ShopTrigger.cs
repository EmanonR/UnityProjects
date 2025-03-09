using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : Interactable
{
    public ShopData shopData;

    public override void Interact()
    {
        if (shopData == null)
        {
            print("There is no shopdata, attach one!"); 
            return;
        }

        if (shopData.itemsInShop.Count == 0)
        {
            print("There is no Items in shopdata, attach some!");
            return;
        }

        foreach (Item item in shopData.itemsInShop)
        {
            print(item.name + "; " + item.value);
        }
    }
}
