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
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject selectionUI;
    [SerializeField] private GameObject craftingUI;
    [SerializeField] private Transform inputSlotsContainer;
    [SerializeField] private SpecificItemSlot outputSlot;
    [SerializeField] private GameObject recipeItemSlotPrefab;
    [SerializeField] private GameObject specificItemSlotPrefab;
    [SerializeField] private Button backToSelectionButton;
    [SerializeField] private Button closeUIButton;
    [SerializeField] private GameObject InventorySlots;

    public Slider craftingProgress;

    [Header("Recipe Selection")]
    public CraftingRecipe[] availableRecipes;
    public Transform recipesContainer;

    private CraftingRecipe currentRecipe;
    private List<SpecificItemSlot> inputSlots = new();
    private bool isUIOpen = false;

    private BuildingCraftingSystem craftingSystem;

    private void Awake()
    {
        craftingSystem = GetComponent<BuildingCraftingSystem>();
        backToSelectionButton.onClick.AddListener(ReturnToSelection);
        closeUIButton.onClick.AddListener(CloseUI);
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
        if (isUIOpen)
        {
            CloseUI();
        }
    }

    public override void interaction()
    {
        if (!isUIOpen)
        {
            OpenUI();
        }
        else
        {
            CloseUI();
        }
    }

    private void OpenUI()
    {
        isUIOpen = true;
        InventorySlots.SetActive(true);
        mainUI.SetActive(true);

        if (currentRecipe != null)
        {
            ShowCraftingUI();
        }
        else
        {
            ShowSelectionUI();
        }
    }

    public void CloseUI()
    {
        isUIOpen = false;
        mainUI.SetActive(false);
        selectionUI.SetActive(false);
        craftingUI.SetActive(false);
        InventorySlots.SetActive(false);
    }

    private void ShowSelectionUI()
    {
        craftingUI.SetActive(false);
        selectionUI.SetActive(true);

        foreach (Transform child in recipesContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var recipe in availableRecipes)
        {
            var slotObj = Instantiate(recipeItemSlotPrefab, recipesContainer);
            var slot = slotObj.GetComponent<RecipeSelectionSlot>();
            slot.SetRecipe(recipe);
            slot.OnClick += SelectRecipe;
        }
    }

    private void ShowCraftingUI()
    {
        selectionUI.SetActive(false);
        craftingUI.SetActive(true);
    }

    public void SelectRecipe(CraftingRecipe recipe)
    {
        currentRecipe = recipe;
        outputSlot.SetAllowedItem(recipe.resultItem);
        ShowCraftingUI();
        SetupSlotsForRecipe(recipe);

        if (CanCraft())
        {
            craftingSystem.QueueCraft(recipe);
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

    private void ReturnToSelection()
    {
        ReturnItemsToInventory();
        currentRecipe = null;
        ShowSelectionUI();
    }

    private void ReturnItemsToInventory()
    {
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