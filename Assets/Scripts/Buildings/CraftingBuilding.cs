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
    [SerializeField] private Slider craftingProgress;
    [SerializeField] private GameObject InventorySlots;

    [Header("Recipe Selection")]
    public CraftingRecipe[] availableRecipes;
    public Transform recipesContainer;

    private CraftingRecipe currentRecipe;
    private bool isCrafting = false;
    private List<SpecificItemSlot> inputSlots = new();
    private float craftingTimer = 0f;
    private bool isUIOpen = false;

    private BuildingCraftingSystem craftingSystem;
    private CraftingBuildingUI ui;

    private void Awake()
    {
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

    private void Update()
    {
        // Автоматический крафт, если есть рецепт
        //if (currentRecipe != null && !isCrafting)
        //{
        //    TryCraft();
        //}

        // Логика крафта

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
        selectionUI.SetActive(false);
        craftingUI.SetActive(false);
        InventorySlots.SetActive(false);
        ReturnItemsToInventory();
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

        TryCraft();
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

    private bool CanCraft()
    {
        if (currentRecipe == null) return false;

        for (int i = 0; i < currentRecipe.ingredients.Count; i++)
        {
            var ingredient = currentRecipe.ingredients[i];
            if (i >= inputSlots.Count ||
                inputSlots[i].Item != ingredient.item ||
                inputSlots[i].Amount < ingredient.amount)
            {
                return false;
            }
        }

        if (outputSlot.Item != null &&
            (outputSlot.Item != currentRecipe.resultItem ||
             outputSlot.EmptyAmount < currentRecipe.resultAmount))
        {
            return false;
        }

        return true;
    }

    private void StartCrafting()
    {
        isCrafting = true;
        craftingTimer = 0f;

        if (craftingProgress != null)
        {
            craftingProgress.gameObject.SetActive(true);
            craftingProgress.value = 0f;
        }
    }

    private void CompleteCrafting()
    {
        for (int i = 0; i < currentRecipe.ingredients.Count; i++)
        {
            if (i < inputSlots.Count)
            {
                inputSlots[i].RemoveAmount(currentRecipe.ingredients[i].amount);
            }
        }

        if (outputSlot.Item == null)
        {
            outputSlot.Set(currentRecipe.resultItem, currentRecipe.resultAmount);
        }
        else
        {
            outputSlot.AddAmount(currentRecipe.resultAmount);
        }

        isCrafting = false;
        craftingTimer = 0f;

        if (craftingProgress != null)
        {
            craftingProgress.gameObject.SetActive(false);
        }
    }

    private void ReturnToSelection()
    {
        ReturnItemsToInventory();
        //autoCraftToggle.isOn = false;
        currentRecipe = null;
        ShowSelectionUI();
    }

    private void ReturnItemsToInventory()
    {
        foreach (var slot in inputSlots)
        {
            if (!slot.IsEmpty)
            {
                int remaining = InventoryManager.Instance.AddItemsToInventory(slot.Item, slot.Amount);
                if (remaining > 0)
                {
                    slot.Set(slot.Item, remaining);
                }
                else
                {
                    slot.Clear();
                }
            }
        }

        if (!outputSlot.IsEmpty)
        {
            int remaining = InventoryManager.Instance.AddItemsToInventory(outputSlot.Item, outputSlot.Amount);
            if (remaining > 0)
            {
                outputSlot.Set(outputSlot.Item, remaining);
            }
            else
            {
                outputSlot.Clear();
            }
        }
    }

    private void TryCraft()
    {
        if (CanCraft())
        {
            StartCrafting();
        }
    }

    private void OnDestroy()
    {
        ReturnItemsToInventory();
    }
}