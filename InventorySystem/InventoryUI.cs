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
            tabs[i].gameObject.SetActive(false);
        }
        tabs[0].gameObject.SetActive(true);

        InventoryManager.itemAdded += UpdateUI;
    }

    void UpdateUI(Item item)
    {
        InventoryManager inventoryManager = GameManager.instance.inventoryManager;

        switch (item.itemType)
        {
            case Item.ItemType.normal:
                //Delete All
                DeleteAllSlotsInList(itemSlots);
                //Add All
                for (int i = 0; i < inventoryManager.normalItems.Count; i++)
                {
                    AddNewItemSlot(itemSlotParent, itemSlots, inventoryManager.normalItems[i].itemInList, inventoryManager.normalItems[i].amount);
                }
                break;
            case Item.ItemType.keyItem:
                //Delete All
                DeleteAllSlotsInList(keyitemSlots);
                //Add All
                for (int i = 0; i < inventoryManager.keyItems.Count; i++)
                {
                    AddNewItemSlot(keyitemSlotParent, keyitemSlots, inventoryManager.keyItems[i].itemInList, inventoryManager.keyItems[i].amount);
                }
                break;
            case Item.ItemType.money:
                moneyText.text = inventoryManager.money.ToString();
                break;
        }
    }

    void DeleteAllSlotsInList(List<GameObject> slotList)
    {
        if (slotList.Count == 0) return;

        for(int i = slotList.Count -1; i >= 0; i--)
        {
            Destroy(slotList[i].gameObject);
        }

        slotList.Clear();
    }

    void AddNewItemSlot(Transform parent, List<GameObject> slotList, Item item, int amount)
    {
        GameObject newItemSlot = Instantiate(itemSlotPrefab, parent);
        ItemSlot itemslot = newItemSlot.GetComponent<ItemSlot>();

        itemslot.UpdateUI(item.icon, item.name, amount.ToString());

        slotList.Add(newItemSlot);
    }

    public void DissableAllEnableOne(GameObject enable)
    {
        foreach (GameObject tab in tabs)
        {
            tab.SetActive(false);
        }

        enable.SetActive(true);
    }
}
