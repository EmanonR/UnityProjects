using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MenuManager
{
    public GameObject[] tabs;

    public List<GameObject> itemSlots;
    public List<GameObject> keyitemSlots;

    [SerializeField] Transform itemSlotParent;
    [SerializeField] Transform keyitemSlotParent;
    [SerializeField] GameObject itemSlotPrefab;
    [SerializeField] TMP_Text moneyText;

    private void Awake()
    {
        for (int i = 0; i < tabs.Length; i++) 
        { 
            tabs[i].SetActive(false);
        }
        tabs[0].SetActive(true);

        InventoryManager.ItemAdded += UpdateUI;
        InventoryManager.ItemRemoved += UpdateUI;
    }

    void UpdateUI(Item item)
    {
        InventoryManager inventoryManager = GameManager.instance.inventoryManager;

        switch (item.itemType)
        {
            case Item.ItemType.normal:
                UpdateItemSlots(itemSlots, inventoryManager.normalItems, itemSlotParent);
                break;
            case Item.ItemType.keyItem:
                UpdateItemSlots(keyitemSlots, inventoryManager.keyItems, keyitemSlotParent);
                break;
            case Item.ItemType.money:
                UpdateMoneyUI(inventoryManager.money.ToString());
                break;
        }
    }

    void UpdateItemSlots(List<GameObject> slotList, List<ItemSlotClass> inventoryList, Transform listParent)
    {
        DeleteAllSlotsInList(slotList);
        for (int i = 0; i < inventoryList.Count; i++)
        {
            AddNewItemSlot(listParent, slotList, inventoryList[i].itemInList, inventoryList[i].amount);
        }
    }

    void DeleteAllSlotsInList(List<GameObject> slotList)
    {
        if (slotList.Count == 0) return;

        for(int i = slotList.Count -1; i >= 0; i--)
        {
            Destroy(slotList[i]);
        }

        slotList.Clear();
    }

    void AddNewItemSlot(Transform parent, List<GameObject> slotList, Item item, int amount)
    {
        GameObject newItemSlot = Instantiate(itemSlotPrefab, parent);
        ItemSlotScript itemslot = newItemSlot.GetComponent<ItemSlotScript>();

        itemslot.UpdateUI(item, amount);

        slotList.Add(newItemSlot);
    }

    public void UpdateMoneyUI(string moneyCount)
    {
        moneyText.text = "Money: " + moneyCount;
    }

    void RemoveItemSlot(List<GameObject> slotList, GameObject itemSlot)
    {
        slotList.Remove(itemSlot);
    }

    public void DissableAllTabsEnableOne(GameObject enable)
    {
        foreach (GameObject tab in tabs)
        {
            tab.SetActive(false);
        }

        enable.SetActive(true);
    }
}
