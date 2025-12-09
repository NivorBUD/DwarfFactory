using System.Linq;
using UnityEngine;

public class AllowedTypeSlot : InventorySlot
{
    [SerializeField] private ItemType[] allowedTypes;

    public override void Set(ItemScriptableObject item, int amount)
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
        => allowedTypes != null && allowedTypes.Contains(item.itemType);
}
