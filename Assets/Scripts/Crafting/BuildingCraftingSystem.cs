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
        OnCraftStarted += HandleCraftStarted;
    }

    private void HandleCraftStarted(CraftingRecipe recipe)
    {
        // При старте крафта показываем нужный прогресс-бар в зависимости от типа станции
        ShowProgressBarForBuilding();
    }

    private void UpdateProgress(CraftingTask task)
    {
        if (InventoryManager.Instance.OpenedCraftingBuilding != building) return;

        var ui = InventoryManager.Instance.ui;
        
        // Обновляем активный прогресс-бар
        if (ui.craftingProgress != null && ui.craftingProgress.gameObject.activeSelf)
        {
            ui.craftingProgress.value = task.Progress;
            Debug.Log($"[BuildingCraftingSystem] Updating NORMAL progress: {task.Progress}");
        }
        else if (ui.craftingProgressShort != null && ui.craftingProgressShort.gameObject.activeSelf)
        {
            ui.craftingProgressShort.value = task.Progress;
            Debug.Log($"[BuildingCraftingSystem] Updating SHORT progress: {task.Progress}");
        }
        else
        {
            Debug.LogWarning($"[BuildingCraftingSystem] No active progress bar! Normal active: {ui.craftingProgress?.gameObject.activeSelf}, Short active: {ui.craftingProgressShort?.gameObject.activeSelf}");
        }
    }

    private void HandleComplete(CraftingTask task)
    {
        // Сброс прогресса при завершении
        if (InventoryManager.Instance.OpenedCraftingBuilding == building)
        {
            ResetProgressBars();
        }

        // Проверяем очередь и добавляем новый крафт если возможно
        if (building.IsCrafting && building.currentRecipe == task.Recipe && HasRequiredItems(task.Recipe))
        {
            QueueCraft(task.Recipe);
        }
    }

    private void ShowProgressBarForBuilding()
    {
        if (InventoryManager.Instance.OpenedCraftingBuilding != building) return;

        var ui = InventoryManager.Instance.ui;
        bool useShortBar = building.buildingType == CraftingBuildingType.Smeltery;
        
        Debug.Log($"[BuildingCraftingSystem] Building type: {building.buildingType}, useShortBar: {useShortBar}");
        Debug.Log($"[BuildingCraftingSystem] craftingProgress: {ui.craftingProgress != null}, craftingProgressShort: {ui.craftingProgressShort != null}");
        
        if (useShortBar)
        {
            // Для Smeltery используем короткий прогресс-бар
            if (ui.craftingProgressShort != null)
            {
                Debug.Log("[BuildingCraftingSystem] Activating SHORT progress bar");
                ui.craftingProgressShort.gameObject.SetActive(true);
                ui.craftingProgressShort.value = 0f;
            }
            else
            {
                Debug.LogError("[BuildingCraftingSystem] craftingProgressShort is NULL!");
            }
            
            if (ui.craftingProgress != null)
            {
                ui.craftingProgress.gameObject.SetActive(false);
            }
        }
        else
        {
            // Для всех остальных станков используем обычный прогресс-бар
            if (ui.craftingProgress != null)
            {
                Debug.Log("[BuildingCraftingSystem] Activating NORMAL progress bar");
                ui.craftingProgress.gameObject.SetActive(true);
                ui.craftingProgress.value = 0f;
            }
            else
            {
                Debug.LogError("[BuildingCraftingSystem] craftingProgress is NULL!");
            }
            
            if (ui.craftingProgressShort != null)
            {
                ui.craftingProgressShort.gameObject.SetActive(false);
            }
        }
    }

    private void ResetProgressBars()
    {
        var ui = InventoryManager.Instance.ui;
        
        if (ui.craftingProgress != null)
        {
            ui.craftingProgress.value = 0f;
        }
        if (ui.craftingProgressShort != null)
        {
            ui.craftingProgressShort.value = 0f;
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

    // Публичный метод для показа правильного прогресс-бара при выборе рецепта
    public void ShowProgressBar()
    {
        ShowProgressBarForBuilding();
    }
}
