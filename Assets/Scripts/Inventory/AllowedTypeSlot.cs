using System.Linq;
using UnityEngine;

public class AllowedTypeSlot : InventorySlot
{
    [SerializeField] private ItemType allowedType;

    public override void Set(ItemScriptableObject item, int amount = 1)
    {
        if (item == null || IsAllowed(item))
        {
            base.Set(item, amount);
        }
    }

    public override int AddAmount(int amount)
    {
        if (Item != null && IsAllowed(Item))
            return base.AddAmount(amount);

        return 0;
    }

    public bool IsAllowed(ItemScriptableObject item)
        => allowedType == item.itemType;

    public bool SetAllowedType(ItemType type)
        => allowedType == type;
}
