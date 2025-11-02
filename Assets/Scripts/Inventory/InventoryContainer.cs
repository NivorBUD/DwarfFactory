using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryContainer
{
    [SerializeField] protected List<InventorySlot> slots;

    public int Capacity => slots.Count;
    public IReadOnlyList<InventorySlot> Slots => slots;

    public InventoryContainer() 
    {
        slots = new List<InventorySlot>();
    }

    public InventoryContainer(GameObject parent)
    {
        slots = new List<InventorySlot>();
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if (parent.transform.GetChild(i).TryGetComponent(out InventorySlot slot))
            {
                slots.Add(slot);
            }
        }
    }

    public void SetNewSlots(List<InventorySlot> newSlots)
    {
        slots = newSlots;
    }

    public int AddItems(ItemScriptableObject item, int amount)
    {
        // Добавляем в существующие стаки
        foreach (var slot in slots)
        {
            if (amount <= 0) return 0;

            if (!slot.isEmpty && slot.Item == item && slot.Amount < item.maximumAmount)
            {
                int space = item.maximumAmount - slot.Amount;
                int addAmount = Mathf.Min(amount, space);
                slot.AddAmount(addAmount);
                amount -= addAmount;
            }
        }

        // Добавляем в пустые слоты
        foreach (var slot in slots)
        {
            if (amount <= 0) return 0;

            if (slot.isEmpty)
            {
                int placeAmount = Mathf.Min(amount, item.maximumAmount);
                slot.PlaceItem(item, placeAmount);
                amount -= placeAmount;
            }
        }

        // Что не влезло
        return amount;
    }

    public bool TryRemoveItem(ItemScriptableObject item, int amount)
    {
        foreach (var slot in slots)
        {
            if (slot.Item == item)
            {
                int toRemove = Mathf.Min(amount, slot.Amount);
                slot.RemoveAmount(toRemove);
                amount -= toRemove;
                if (amount <= 0) return true;
            }
        }
        return false;
    }

    public bool HasItemsForRecipe(CraftingRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            int total = 0;
            foreach (var slot in slots)
                if (slot.Item == ingredient.item)
                    total += slot.Amount;

            if (total < ingredient.amount)
                return false;
        }
        return true;
    }

    public void RemoveItemsForRecipe(CraftingRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
            TryRemoveItem(ingredient.item, ingredient.amount);
    }
}
