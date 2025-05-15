using System.Collections.Generic;
using UnityEngine;

public class CraftingBuilding : Building
{
    public List<CraftingRecipe> AvailableCrafts;
    public BuildingInventory buildingInventory;
    private Queue<CraftingRecipe> jobQueue = new();
    private bool isCrafting = false;

    public void AddCraftingJob(CraftingRecipe recipe)
    {
        if (HasRequiredItems(recipe))
        {
            foreach (var ingredient in recipe.ingredients)
            {
                buildingInventory.RemoveItems(ingredient.item, ingredient.amount);
            }

            jobQueue.Enqueue(recipe);
            if (!isCrafting)
                StartCoroutine(ProcessCraftingJobs());
        }
        else
        {
            Debug.Log("Недостаточно ресурсов в здании для крафта.");
        }
    }

    private IEnumerator<WaitForSeconds> ProcessCraftingJobs()
    {
        isCrafting = true;
        while (jobQueue.Count > 0)
        {
            var recipe = jobQueue.Dequeue();
            Debug.Log($"Начинается крафт: {recipe.resultItem.name}");

            yield return new WaitForSeconds(recipe.craftingTime);

            InventoryManager.Instance.TryAddItem(recipe.resultItem, recipe.resultAmount);
            Debug.Log($"Закончен крафт: {recipe.resultItem.name}");
        }
        isCrafting = false;
    }

    private bool HasRequiredItems(CraftingRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            if (buildingInventory.CountItem(ingredient.item) < ingredient.amount)
                return false;
        }
        return true;
    }

    public override void interaction()
    {
        setCrafts();
    }

    private void setCrafts()
    {
        foreach (CraftingRecipe recipe in AvailableCrafts) {

        }
    }
}
