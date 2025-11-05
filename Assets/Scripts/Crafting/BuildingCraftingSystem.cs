using UnityEngine;

public class BuildingCraftingSystem : BaseCraftingSystem
{
    [SerializeField] private BuildingInventory buildingInventory;

    protected override bool HasRequiredItems(CraftingRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            if (buildingInventory.CountItem(ingredient.item) < ingredient.amount)
                return false;
        }
        return true;
    }

    protected override void RemoveIngredients(CraftingRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
            buildingInventory.RemoveItems(ingredient.item, ingredient.amount);
    }

    protected override void AddResult(CraftingRecipe recipe)
    {
        buildingInventory.AddItem(recipe.resultItem, recipe.resultAmount);
    }
}
