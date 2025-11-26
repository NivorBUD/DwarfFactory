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

    public ItemScriptableObject Item { get; protected set; }
    public int Amount { get; protected set; }
    public bool IsEmpty => Item == null;

    public int EmptyAmount => Item ? Item.maximumAmount - Amount : 0;

    //public bool IsEmpty = true;

    public virtual InventorySlot Copy()
    {
        InventorySlot newSlot = new();
        newSlot.BGImage = BGImage;
        newSlot.iconImage = iconImage;
        newSlot.textAmount = textAmount;
        newSlot.Item = Item;
        newSlot.Amount = Amount;

        return newSlot;
    }

    private void Awake()
    {
        if (!BGImage) BGImage = GetComponent<Image>();
        if (!iconImage) iconImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        if (!textAmount) textAmount = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        Clear();
    }

    public virtual void Set(ItemScriptableObject item, int amount)
    {
        Item = item;
        Amount = amount;

        if (item == null || amount == 0)
        {
            Clear();
            return;
        }

        if (iconImage != null)
        {
            iconImage.sprite = item.icon;
            iconImage.color = new Color(1, 1, 1, 1);
        }
        if (textAmount != null)
        {
            textAmount.text = amount > 1 ? amount.ToString() : "";
        }
    }

    public virtual void Clear()
    {
        Item = null;
        Amount = 0;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.color = new Color(1, 1, 1, 0);
        }
        if (textAmount != null)
        {
            textAmount.text = "";
        }
    }

    public void UpdateAmount(int newAmount)
    {
        if (newAmount <= 0)
        {
            Clear();
            return;
        }
        Amount = newAmount;
        textAmount.text = newAmount > 1 ? newAmount.ToString() : "";
    }

    public virtual int AddAmount(int addedAmount)
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
