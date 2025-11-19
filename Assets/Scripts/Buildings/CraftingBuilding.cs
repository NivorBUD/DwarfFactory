using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public enum CraftingBuildingType
{
    Mine,
    Smeltery,
    Taanning,
    Workshop,
    WoodBreaker
}

public class CraftingBuilding : Building
{
    public CraftingBuildingType buildingType;

    [Header("UI References")]
    [SerializeField] private Transform inputSlotsContainer;
    [SerializeField] private SpecificItemSlot outputSlot;
    [SerializeField] private GameObject recipeItemSlotPrefab;
    [SerializeField] private GameObject specificItemSlotPrefab;
    [SerializeField] private GameObject InventorySlots;

    public Slider craftingProgress;

    [Header("Recipe Selection")]
    public List<CraftingRecipe> AvailableRecipes { get; private set; }

    private CraftingRecipe currentRecipe;
    private List<SpecificItemSlot> inputSlots = new();
    public bool isSelectionCraft { get; private set; }

    private BuildingCraftingSystem craftingSystem;

    private void Awake()
    {
        craftingSystem = GetComponent<BuildingCraftingSystem>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnInventoryToggle += HandleInventoryToggleInCrafting;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnInventoryToggle -= HandleInventoryToggleInCrafting;
        }
    }

    private void HandleInventoryToggleInCrafting()
    {
        if (InventoryManager.Instance.ui.IsCraftingBuildingOpened)
        {
            InventoryManager.Instance.OpenCraftingBuilding(this);
        }
    }

    public override void interaction()
    {
        InventoryManager.Instance.OpenCraftingBuilding(this);
    }

    public void SelectRecipe(CraftingRecipe recipe)
    {
        isSelectionCraft = false;
        currentRecipe = recipe;
        outputSlot.SetAllowedItem(recipe.resultItem);
        SetupSlotsForRecipe(recipe);

        if (CanCraft())
        {
            craftingSystem.QueueCraft(recipe);
        }

        InventoryManager.Instance.ui.ShowCraftCraftingBuildingUI();
        
    }

    public void SetupRecipesSlots(GameObject recipesContainer)
    {
        isSelectionCraft = true;
        foreach (Transform child in recipesContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var recipe in AvailableRecipes)
        {
            var slotObj = Instantiate(recipeItemSlotPrefab, recipesContainer.transform);
            var slot = slotObj.GetComponent<RecipeSelectionSlot>();
            slot.SetRecipe(recipe);
            slot.OnClick += SelectRecipe;
        }
    }

    private void SetupSlotsForRecipe(CraftingRecipe recipe)
    {
        foreach (var slot in inputSlots)
        {
            Destroy(slot.gameObject);
        }

        inputSlots.Clear();
        outputSlot.Clear();

        for (int i = 0; i < recipe.ingredients.Count; i++)
        {
            var slotObj = Instantiate(specificItemSlotPrefab, inputSlotsContainer);
            var slot = slotObj.GetComponent<SpecificItemSlot>();
            slot.SetAllowedItem(recipe.ingredients[i].item);
            inputSlots.Add(slot);
        }
    }

    public bool HasItemInInputSlots(ItemScriptableObject item, int amount)
    {
        int total = 0;
        foreach (var slot in inputSlots)
        {
            if (slot.Item == item)
                total += slot.Amount;
        }
        return total >= amount;
    }

    public void ConsumeInputItems(CraftingRecipe recipe)
    {
        for (int i = 0; i < recipe.ingredients.Count; i++)
        {
            var ing = recipe.ingredients[i];
            int remain = ing.amount;
            foreach (var slot in inputSlots)
            {
                if (slot.Item == ing.item && remain > 0)
                {
                    int taken = Mathf.Min(slot.Amount, remain);
                    slot.RemoveAmount(taken);
                    remain -= taken;
                }
            }
        }
    } 

    public bool CanOutput(CraftingRecipe recipe)
    {
        if (outputSlot.IsEmpty)
            return true;

        if (outputSlot.Item != recipe.resultItem)
            return false;

        return outputSlot.EmptyAmount >= recipe.resultAmount;
    }

    public void AddOutputItem(CraftingRecipe recipe)
    {
        if (outputSlot.IsEmpty)
            outputSlot.Set(recipe.resultItem, recipe.resultAmount);
        else
            outputSlot.AddAmount(recipe.resultAmount);
    }

    public void ReturnItemsToInventory()
    {
        currentRecipe = null;
        foreach (var slot in inputSlots)
        {
            if (!slot.IsEmpty)
            {
                int remain = InventoryManager.Instance.AddItemsToInventory(slot.Item, slot.Amount);
                if (remain <= 0) 
                    slot.Clear();
            }
        }
        if (!outputSlot.IsEmpty)
        {
            int remain = InventoryManager.Instance.AddItemsToInventory(outputSlot.Item, outputSlot.Amount);
            if (remain <= 0) 
                outputSlot.Clear();
        }
    }

    private bool CanCraft()
    {
        return craftingSystem != null && craftingSystem.enabled && HasAllIngredients() && CanOutput(currentRecipe);
    }

    private bool HasAllIngredients()
    {
        foreach (var ing in currentRecipe.ingredients)
            if (!HasItemInInputSlots(ing.item, ing.amount))
                return false;
        return true;
    }

    private void OnDestroy()
    {
        ReturnItemsToInventory();
    }
}