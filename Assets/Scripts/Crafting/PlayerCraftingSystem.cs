using UnityEngine;

public class PlayerCraftingSystem : BaseCraftingSystem
{
    public static PlayerCraftingSystem Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    protected override bool HasRequiredItems(CraftingRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            if (InventoryManager.Instance.CountItem(ingredient.item) < ingredient.amount)
                return false;
        }
        return true;
    }

    protected override void RemoveIngredients(CraftingRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
            InventoryManager.Instance.RemoveItems(ingredient.item, ingredient.amount);
    }

    protected override void AddResult(CraftingRecipe recipe)
    {
        InventoryManager.Instance.AddItemsToInventory(recipe.resultItem, recipe.resultAmount);
    }
}