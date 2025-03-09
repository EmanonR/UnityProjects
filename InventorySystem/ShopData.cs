using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop", menuName = "Custom/ShopData")]
public class ShopData : ScriptableObject
{
    public List<Item> itemsInShop;
}
