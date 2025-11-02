using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    protected Image BGImage;
    protected Image iconImage;
    protected TextMeshProUGUI textAmount;

    public ItemScriptableObject Item { get; private set; }
    public int Amount { get; private set; }
    public bool IsEmpty => Item == null;

    public int EmptyAmount => Item ? Item.maximumAmount - Amount : 0;

    //public bool IsEmpty = true;

    private void Awake()
    {
        if (!BGImage) BGImage = GetComponent<Image>();
        if (!iconImage) iconImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        if (!textAmount) textAmount = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        Clear();
    }

    public void Set(ItemScriptableObject item, int amount)
    {
        Item = item;
        Amount = amount;

        if (item == null || amount == 0)
        {
            Clear();
            return;
        }

        iconImage.sprite = item.icon;
        iconImage.color = new Color(1, 1, 1, 1);
        textAmount.text = amount > 1 ? amount.ToString() : "";
    }

    public void Clear()
    {
        Item = null;
        Amount = 0;

        iconImage.sprite = null;
        iconImage.color = new Color(1, 1, 1, 0);
        textAmount.text = "";
    }

    public void UpdateAmount(int newAmount)
    {
        Amount = newAmount;
        textAmount.text = newAmount > 1 ? newAmount.ToString() : "";
    }

    public int AddAmount(int addedAmount)
    {
        if (Amount + addedAmount > Item.maximumAmount)
        {
            UpdateAmount(Item.maximumAmount);
            return Item.maximumAmount - (Amount + addedAmount);
        } 
        else
        {
            UpdateAmount(Amount + addedAmount);
        }
        
        return 0;
    }

    public int RemoveAmount(int removeAmount)
    {
        if (removeAmount > Amount)
        {
            Clear();
            return removeAmount - Amount;
        }
        else
        {
            UpdateAmount(Amount - removeAmount);
        }

        return 0;
    }

    /// <summary>
    /// Смена фона слота, для активных слотов в панели быстрых слотов
    /// </summary>
    public void ChangBG(Color color)
    {
        BGImage.color = color;
    }

    /// <summary>
    /// Смена фона слота на стандартный, для активных слотов в панели быстрых слотов
    /// </summary>
    public void ChangeBGToStandard()
    {
        BGImage.color = new Color(1, 1, 1, 1);
    }
}
