using UnityEngine;
using UnityEngine.UI;

public class SpecificItemSlot : InventorySlot
{
    [SerializeField] private ItemScriptableObject allowedItem;
    [SerializeField] private Image hintIcon;
    [SerializeField] private bool isOutputSlot = false; // Маркер выходного слота

    public ItemScriptableObject AllowedItem => allowedItem;
    public bool IsOutputSlot => isOutputSlot;

    public override void Set(ItemScriptableObject item, int amount) 
    {
        if (item == null || item == allowedItem)
        {
            base.Set(item, amount);
        }

        
    }

    public override int AddAmount(int amount)
    {
        if (Item == allowedItem)
        {
            return base.AddAmount(amount);
        }
        return 0;
    }

    public void SetAllowedItem(ItemScriptableObject item)
    {
        allowedItem = item;
        if (allowedItem && hintIcon)
        {
            hintIcon.sprite = allowedItem.icon;
            hintIcon.color = new Color(1, 1, 1, 0.5f); // �������������� ���������
        }
    }
    
    public void SetAsOutputSlot(bool isOutput)
    {
        isOutputSlot = isOutput;
    }

    public override InventorySlot Copy()
    {
        SpecificItemSlot newSlot = new ();
        
        newSlot.Amount = base.Amount;
        newSlot.BGImage = base.BGImage;
        newSlot.iconImage = base.iconImage;
        newSlot.textAmount = base.textAmount;
        newSlot.allowedItem = allowedItem;
        
        return newSlot;
    }

    public override void Clear()
    {
        base.Clear();
    }
}
