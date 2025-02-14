using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image itemSprite;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text amountText;

    public Item iteminSlot;
    public int amount;

    public void UpdateUI(Sprite sprite, string name, string amount)
    {
        amountText.text = amount;
        nameText.text = name;
        itemSprite.sprite = sprite;
    }
}
