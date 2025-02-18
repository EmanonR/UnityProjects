using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Progress;

public class ItemSlotScript : MonoBehaviour
{
    [SerializeField] Image itemSprite;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text amountText;

    public Item itemInSlot;
    public int amount;

    public void UpdateUI(Item newItem, int amount)
    {
        amountText.text = amount.ToString();
        nameText.text = newItem.name;
        itemSprite.sprite = newItem.icon;
        itemInSlot = newItem;
    }
}
