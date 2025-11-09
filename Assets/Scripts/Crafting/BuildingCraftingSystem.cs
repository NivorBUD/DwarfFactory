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
        if (building.craftingProgress != null)
        {
            building.craftingProgress.value = task.Progress;
        }
    }

    private void HandleComplete(CraftingTask task)
    {
        // Скрыть прогресс при завершении
        if (building.craftingProgress != null)
            building.craftingProgress.value = 0f;

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



    /// <summary>
    /// Очистить очередь и остановить обработку.
    /// </summary>
    //public void ClearQueue()
    //{
    //    craftingQueue.Clear();
    //    OnQueueCountChanged(0);
    //    if (isCrafting)
    //    {
    //        StopCoroutine(processCoroutine);
    //        processCoroutine = null;
    //    }
    //}
}
