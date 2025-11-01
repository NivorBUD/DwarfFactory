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
    [SerializeField]
    private GameObject mainUI;
    [SerializeField]
    private GameObject selectionUI;
    [SerializeField]
    private GameObject craftingUI;
    [SerializeField]
    private Transform inputSlotsContainer;
    [SerializeField]
    private SpecificItemSlot outputSlot;
    [SerializeField]
    private GameObject recipeItemSlotPrefab;
    [SerializeField]
    private GameObject specificItemSlotPrefab;
    [SerializeField]
    private Button backToSelectionButton;
    [SerializeField]
    private Button closeUIButton;
    [SerializeField]
    private Slider craftingProgress;
    [SerializeField]
    private GameObject InventorySlots;
    //public Toggle autoCraftToggle;

    [Header("Recipe Selection")]
    public CraftingRecipe[] availableRecipes;
    public Transform recipesContainer;

    private CraftingRecipe currentRecipe;
    private bool isCrafting = false;
    private List<SpecificItemSlot> inputSlots = new List<SpecificItemSlot>();
    private float craftingTimer = 0f;
    private bool isUIOpen = false;
    private bool isAutoCraftEnabled = false; 

    private void Awake()
    {
        backToSelectionButton.onClick.AddListener(ReturnToSelection);
        //autoCraftToggle.onValueChanged.AddListener(SetAutoCraft);
    }

    private void Update()
    {
        if (isUIOpen && Input.GetKeyDown(KeyCode.E))
        {
            CloseUI();
        }

        // Автоматический крафт, если включен и есть рецепт
        if (isAutoCraftEnabled && currentRecipe != null && !isCrafting)
        {
            TryAutoCraft();
        }

        // Логика крафта
        if (isCrafting)
        {
            craftingTimer += Time.deltaTime;

            if (craftingProgress != null)
            {
                craftingProgress.value = Mathf.Clamp01(craftingTimer / currentRecipe.craftingTime);
            }

            if (craftingTimer >= currentRecipe.craftingTime)
            {
                CompleteCrafting();
            }
        }
    }

    private void TryAutoCraft()
    {
        if (CanCraft())
        {
            StartCrafting();
        }
    }

    public void SetAutoCraft(bool enabled)
    {
        isAutoCraftEnabled = enabled;
        // Если включаем автокрафт и есть рецепт - сразу пробуем крафтить
        if (enabled && currentRecipe != null)
        {
            TryAutoCraft();
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
            // Обновляем состояние переключателя при открытии
            //autoCraftToggle.isOn = isAutoCraftEnabled;
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

        // При выборе нового рецепта включаем автокрафт
        isAutoCraftEnabled = true;
        //autoCraftToggle.isOn = true;
        TryAutoCraft();
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

        // После завершения крафта сразу пробуем крафтить снова, если автокрафт включен
        if (isAutoCraftEnabled)
        {
            TryAutoCraft();
        }
    }

    private void ReturnToSelection()
    {
        ReturnItemsToInventory();
        isAutoCraftEnabled = false;
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

    private void OnDestroy()
    {
        ReturnItemsToInventory();
    }
}