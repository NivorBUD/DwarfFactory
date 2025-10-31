using System.Collections.Generic;
using UnityEngine;

public class BuildingInventory
{
    public List<SpecificItemSlot> slots;
    public InventorySlot resultSlot;

    public int CountItem(ItemScriptableObject item)
    {
        int count = 0;
        foreach (var slot in slots)
        {
            if (slot.Item == item)
                count += slot.Amount;
        }
        return count;
    }

    public void RemoveItems(ItemScriptableObject item, int amount)
    {
        foreach (var slot in slots)
        {
            if (slot.Item == item)
            {
                int remove = Mathf.Min(slot.Amount, amount);
                slot.RemoveAmount(remove);
                amount -= remove;
                if (amount <= 0) return;
            }
        }
    }

    public void AddItem(ItemScriptableObject item, int amount)
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.Set(item, amount);
                return;
            }
            if (slot.Item == item && slot.Amount < item.maximumAmount)
            {
                int add = Mathf.Min(item.maximumAmount - slot.Amount, amount);
                slot.AddAmount(add);
                amount -= add;
                if (amount <= 0) return;
            }
        }
    }
}
