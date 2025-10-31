using UnityEngine;

public class SpecificItemSlot : InventorySlot
{
    [SerializeField] private ItemScriptableObject allowedItem;

    public ItemScriptableObject AllowedItem => allowedItem;

    public new void Set(ItemScriptableObject item, int amount)
    {
        if (item == null || item != allowedItem)
        {
            return;
        }

        base.Set(item, amount);
    }

    public new void AddAmount(int amount)
    {
        if (Item == allowedItem)
        {
            base.AddAmount(amount);
        }
    }

    public void SetAllowedItem(ItemScriptableObject item)
    {
        allowedItem = item;
    }
}
