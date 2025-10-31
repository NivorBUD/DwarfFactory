using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance;

    [SerializeField] private List<CraftingSlot> CraftingSlots;

    private void Awake()
    {
        Instance = this;
    }

    public bool Craft(CraftingRecipe recipe)
    {
        if (!HasRequiredItems(recipe))
            return false;

        StartCoroutine(CraftDelay(recipe));

        
        return true;
    }

    private IEnumerator CraftDelay(CraftingRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            InventoryManager.Instance.RemoveItems(ingredient.item, ingredient.amount);
        }

        CalculateMaxInventoryCrafts();

        yield return new WaitForSeconds(recipe.craftingTime);

        InventoryManager.Instance.AddItemsToInventory(recipe.resultItem, recipe.resultAmount);
        CalculateMaxInventoryCrafts();
    }

    private bool HasRequiredItems(CraftingRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            int playerAmount = InventoryManager.Instance.CountItem(ingredient.item);
            if (playerAmount < ingredient.amount)
                return false;
        }
        return true;
    }

    public int CalculateMaxCrafts(CraftingRecipe recipe)
    {
        int maxCrafts = -1;
        foreach (var ingredient in recipe.ingredients)
        {
            int playerAmount = InventoryManager.Instance.CountItem(ingredient.item);
            int craftsForThisItem = playerAmount / ingredient.amount;
            if (craftsForThisItem < maxCrafts || maxCrafts == -1)
                maxCrafts = craftsForThisItem;
        }
        return maxCrafts;
    }

    public void CalculateMaxInventoryCrafts()
    {
        foreach (CraftingSlot slot in CraftingSlots)
        {
            slot.RefreshAmountText();
        }
    }
}
