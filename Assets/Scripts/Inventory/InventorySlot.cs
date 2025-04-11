using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public ItemScriptableObject Item { get; private set; }
    public int Amount { get; private set; }

    public int EmptyAmount => Item ? Item.maximumAmount - Amount : 0;

    public bool isEmpty = true;
    public Image iconImage;
    public Image BGImage;
    public TextMeshProUGUI textAmount;

    private void Awake()
    {
        BGImage = GetComponent<Image>();
        iconImage = transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>();
        textAmount = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        textAmount.text = "";
    }

    public void SetIcon(Sprite icon)
    {
        iconImage.sprite = icon;
        iconImage.color = new Color(1, 1, 1, 1);
    }

    public void AddAmount(int amount)
    {
        Amount += amount;
        textAmount.text = Amount.ToString();

        if (GetComponent<ChestSlot>() != null)
        {
            GetComponent<ChestSlot>().UpdateInfo(Item, Amount);
        }
    }

    public void RemoveAmount(int amount)
    {
        Amount -= amount;
        if (Amount <= 0)
        {
            ResetData();
        }
        textAmount.text = Amount == 0 ? "" : Amount.ToString();

        if (GetComponent<ChestSlot>() != null)
        {
            GetComponent<ChestSlot>().UpdateInfo(Item, Amount);
        }    
    }

    public void PlaceItem(ItemScriptableObject item, int amount)
    {
        if (amount == 0)
        {
            ResetData();
            return;
        }

        isEmpty = false;
        Item = item;
        Amount = amount;
        textAmount.text = Amount == 0 ? "" : Amount.ToString();
        if (item == null) 
            SetIcon(null);
        else
            SetIcon(item.icon);

        if (GetComponent<ChestSlot>() != null)
            GetComponent<ChestSlot>().UpdateInfo(Item, Amount);
    }

    public void ResetData()
    {
        Item = null;
        Amount = 0;
        isEmpty = true;
        iconImage.sprite = null;
        iconImage.color = new Color(1, 1, 1, 0);
        textAmount.text = "";
        
        if (GetComponent<ChestSlot>() != null)
            GetComponent<ChestSlot>().UpdateInfo(Item, Amount);
    }
}
