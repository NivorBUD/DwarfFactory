using UnityEngine;

public class SpecificItemSlot : InventorySlot
{
    [SerializeField] private ItemScriptableObject allowedItem;

    public ItemScriptableObject AllowedItem => allowedItem;

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
