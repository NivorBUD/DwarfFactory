using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCraftingSystem : BaseCraftingSystem
{
    public int GetQueueCount() => craftingQueue.Count;

    public CraftingTask GetCurrentTask() => craftingQueue.Count > 0 ? craftingQueue.Peek() : null;

    private Coroutine processCoroutine;

    private CraftingBuilding building;

    private void Awake()
    {
        OnCraftProgress += ProgressCraft;
        OnCraftCompleted += CompleteCraft;
        if (!TryGetComponent<CraftingBuilding>(out building))
        {
            Debug.LogError("[BuildingCraftingSystem] Не найден CraftingBuilding на объекте!");
        }
    }

    /// <summary>
    /// Очистить очередь и остановить обработку.
    /// </summary>
    public void ClearQueue()
    {
        craftingQueue.Clear();
        OnQueueCountChanged(0);
        if (isCrafting)
        {
            StopCoroutine(processCoroutine);
            processCoroutine = null;
        }
    }

    
    private void ProgressCraft(CraftingTask task)
    {

    }

    private void CompleteCraft(CraftingTask task)
    {

    }

    protected override bool HasRequiredItems(CraftingRecipe recipe)
    {
        if (building == null) return false;

        foreach (var ingredient in recipe.ingredients)
        {
            if (building.CountItem(ingredient.item) < ingredient.amount)
                return false;
        }
        return true;
    }

    protected override void RemoveIngredients(CraftingRecipe recipe)
    {
        if (building == null) return;

        foreach (var ingredient in recipe.ingredients)
            building.RemoveItems(ingredient.item, ingredient.amount);
    }

    protected override void AddResult(CraftingRecipe recipe)
    {
        if (building == null) return;
        building.AddItem(recipe.resultItem, recipe.resultAmount);
    }
}
