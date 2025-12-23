using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UIElements;

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
    [SerializeField] private SpecificItemSlot outputSlot;
    [SerializeField] private GameObject recipeItemSlotPrefab;
    [SerializeField] private GameObject specificItemSlotPrefab;

    [Header("Recipe Selection")]
    [SerializeField] private List<CraftingRecipe> AvailableRecipes;

    private CraftingRecipe currentRecipe;
    private List<SpecificItemSlot> inputSlots = new();
    public bool IsCrafting { get; private set; }

    private BuildingCraftingSystem craftingSystem;

    private void Awake()
    {
        craftingSystem = GetComponent<BuildingCraftingSystem>();
        outputSlot = new();
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
        //if (InventoryManager.Instance.ui.IsCraftingBuildingOpened)
        //{
        //    InventoryManager.Instance.OpenCraftingBuilding(this);
        //}
    }

    public override void interaction()
    {
        InventoryManager.Instance.OpenCraftingBuilding(this);
    }

    public void SelectRecipe(CraftingRecipe recipe)
    {
        currentRecipe = recipe;
        outputSlot.SetAllowedItem(recipe.resultItem);
        InventoryManager.Instance.ui.ChangeCraftAndSelectionCraftingBuilding();

        IsCrafting = true;

        if (CanCraft())
        {
            craftingSystem.QueueCraft(recipe);
        }
    }

    private void FixedUpdate()
    {
        if (InventoryManager.Instance.OpenedCraftingBuilding == this && InventoryManager.Instance.ui.IsCraftingBuildingOpened)
        {
            SaveData();
        }
        

        if (IsCrafting)
        {
            craftingSystem.TryStartCrafting(currentRecipe);
        }
    }

    public void SetupRecipesSlots(GameObject recipesContainer)
    {
        IsCrafting = false;
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

    public void InizializeUICraftingSlots()
    {
        foreach (var slot in InventoryManager.Instance.ui.BuildingInputSlotsContainer.gameObject.GetComponentsInChildren<SpecificItemSlot>())
        {
            Destroy(slot.gameObject);
        }

        if (InventoryManager.Instance.ui.BuildingOutputSlotObject.GetComponentInChildren<SpecificItemSlot>())
        {
            Destroy(InventoryManager.Instance.ui.BuildingOutputSlotObject.GetComponentInChildren<SpecificItemSlot>().gameObject);
        }

        List<SpecificItemSlot> slots = new();
        foreach (RecipeIngredient ingridient in currentRecipe.ingredients)  
        {
            var slotObj = Instantiate(specificItemSlotPrefab, InventoryManager.Instance.ui.BuildingInputSlotsContainer);
            var slot = slotObj.GetComponent<SpecificItemSlot>();
            slot.SetAllowedItem(ingridient.item);
            slots.Add(slot);
        }

        SpecificItemSlot outSlot = Instantiate(specificItemSlotPrefab, InventoryManager.Instance.ui.BuildingOutputSlotObject).GetComponent<SpecificItemSlot>();
        outSlot.SetAllowedItem(outputSlot.AllowedItem);

        outSlot.Set(outputSlot.Item, outputSlot.Amount);

        bool isSlotsSet = inputSlots.Count == slots.Count;
        List<SpecificItemSlot> slotsToSave = new();
        for (int i = 0; i < slots.Count; i++)
        {
            if (isSlotsSet)
            {
                slots[i].Set(inputSlots[i].Item, inputSlots[i].Amount);
            }
            else
            {
                slotsToSave.Add((SpecificItemSlot)slots[i].Copy());
            }
        }
        if (!isSlotsSet)
        {
            inputSlots = slotsToSave;
        }
    }

    public void SaveData()
    {
        List<SpecificItemSlot> newSlots = new(InventoryManager.Instance.ui.BuildingInputSlotsContainer.GetComponentsInChildren<SpecificItemSlot>());
        for (int i = 0; i < inputSlots.Count; i++)
        {
            inputSlots[i].Set(newSlots[i].Item, newSlots[i].Amount);
        }

        SpecificItemSlot outUiSlot = InventoryManager.Instance.ui.BuildingOutputSlotObject.GetComponentInChildren<SpecificItemSlot>();
        for (int i = 0; i < inputSlots.Count; i++)
        {
            outputSlot.Set(outUiSlot.Item, outUiSlot.Amount);
        }
    }

    public void DownloadDataInUi()
    {
        List<SpecificItemSlot> newSlots = new(InventoryManager.Instance.ui.BuildingInputSlotsContainer.GetComponentsInChildren<SpecificItemSlot>());
        for (int i = 0; i < inputSlots.Count; i++)
        {
            newSlots[i].Set(inputSlots[i].Item, inputSlots[i].Amount);
        }

        SpecificItemSlot outUiSlot = InventoryManager.Instance.ui.BuildingOutputSlotObject.GetComponentInChildren<SpecificItemSlot>();
        outUiSlot.Clear();
        outUiSlot.SetAllowedItem(outputSlot.Item);
        outUiSlot.Set(outputSlot.Item, outputSlot.Amount);
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
                    if (InventoryManager.Instance.OpenedCraftingBuilding == this)
                    {
                        DownloadDataInUi();
                    }
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

        if (InventoryManager.Instance.OpenedCraftingBuilding == this)
        {
            DownloadDataInUi();
        }
    }

    public void ReturnItemsToPlayerInventory()
    {
        craftingSystem.ClearQueue();
        currentRecipe = null;

        //if (!outputSlot)
        //{
        //    return;
        //}

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
        inputSlots.Clear();
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
        ReturnItemsToPlayerInventory();
    }
}