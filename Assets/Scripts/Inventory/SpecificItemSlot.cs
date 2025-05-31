using UnityEngine;

public class SpecificItemSlot : InventorySlot
{
    [SerializeField] private ItemScriptableObject allowedItem;

    public ItemScriptableObject AllowedItem => allowedItem;

    public new void PlaceItem(ItemScriptableObject item, int amount)
    {
        if (item == null || item != allowedItem)
        {
            return;
        }

        base.PlaceItem(item, amount);
    }

    public new void AddAmount(int amount)
    {
        if (Item == allowedItem)
        {
            base.AddAmount(amount);
        }
    }
}
