using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCraftingSystem : BaseCraftingSystem
{
    private CraftingBuilding building;

    private void Awake()
    {
        if (!TryGetComponent<CraftingBuilding>(out building))
        {
            Debug.LogError("[BuildingCraftingSystem] Не найден CraftingBuilding на объекте!");
        }

        OnCraftProgress += UpdateProgress;
        OnCraftCompleted += HandleComplete;
    }

    private void UpdateProgress(CraftingTask task)
    {
        if (InventoryManager.Instance.ui.craftingProgress != null && InventoryManager.Instance.OpenedCraftingBuilding == building)
        {
            InventoryManager.Instance.ui.craftingProgress.value = task.Progress;
        }
    }

    private void HandleComplete(CraftingTask task)
    {
        // Скрыть прогресс при завершении
        if (InventoryManager.Instance.OpenedCraftingBuilding == building && InventoryManager.Instance.ui.craftingProgress != null)
            InventoryManager.Instance.ui.craftingProgress.value = 0f;

        // Проверяем ресурсы — повторяем крафт если возможно
        if (HasRequiredItems(task.Recipe))
        {
            QueueCraft(task.Recipe);
        }
    }

    protected override bool HasRequiredItems(CraftingRecipe recipe)
    {
        if (building == null) return false;

        foreach (var ingredient in recipe.ingredients)
        {
            if (!building.HasItemInInputSlots(ingredient.item, ingredient.amount))
                return false;
        }
        if (!building.CanOutput(recipe))
            return false;
        
        return true;
    }

    protected override void RemoveIngredients(CraftingRecipe recipe)
    {
        building.ConsumeInputItems(recipe);
    }

    protected override void AddResult(CraftingRecipe recipe)
    {
        building.AddOutputItem(recipe);
    }

    public void TryStartCrafting(CraftingRecipe recipe)
    {
        if (recipe == null) return;

        if (craftingQueue.Count != 0) return;

        if (HasRequiredItems(recipe))
        {
            QueueCraft(recipe);
        }
    }
}
