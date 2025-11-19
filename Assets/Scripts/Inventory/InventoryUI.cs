using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Crafting Building References")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject selectionUI;
    [SerializeField] private GameObject craftingUI;
    [SerializeField] private Transform inputSlotsContainer;
    [SerializeField] private SpecificItemSlot outputSlot;
    [SerializeField] private GameObject recipeItemSlotPrefab;
    [SerializeField] private GameObject specificItemSlotPrefab;
    [SerializeField] private GameObject recipesContainer;


    [SerializeField] private Button backToSelectionButton;
    [SerializeField] private Button closeUIButton;

    [Header("UI Player References")]
    [SerializeField] private GameObject InventoryPanel;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject chestInventory;
    [SerializeField] private GameObject quickSlots;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private GameObject craftingPanel;

    public bool IsInventoryOpened => inventory.activeSelf;
    public bool IsChestOpened { get; private set; }
    public bool IsCraftingBuildingOpened { get; private set; }

    private InventoryContainer container;

    public void Initialize(InventoryContainer container)
    {
        this.container = container;
        //Refresh();
    }

    public void UpdateSlot(int index, ItemScriptableObject item, int amount)
    {
        if (index >= 0 && index < container.Slots.Count)
        {
            container.Slots[index].Set(item, amount);
        }
    }

    public void ToggleInventory()
    {
        if (IsCraftingBuildingOpened)
        {
            if (InventoryManager.Instance.OpenedCraftingBuilding.isSelectionCraft)
            {
                ShowSelectionCraftingBuildingUI();
            }
            else
            {
                ShowCraftCraftingBuildingUI();
            }
        }
        else 
        {
            closeButton.SetActive(!IsInventoryOpened);
            if (IsChestOpened)
            {
                chestInventory.SetActive(!IsInventoryOpened);
            } 
            else
            {
                chestInventory.SetActive(false);
            }
            craftingPanel.SetActive(!chestInventory.activeSelf);

            
            InventoryPanel.SetActive(!IsInventoryOpened);
        }


        
        if (IsInventoryOpened)
        {
            if (IsChestOpened)
            {
                InventoryManager.Instance.SaveChestInventory();
                IsChestOpened = false;
            }
            if (IsCraftingBuildingOpened)
            {

                IsCraftingBuildingOpened = false;
            }
        }

        // последним, потому что на него ориентируется IsInventoryOpened
        inventory.SetActive(!IsInventoryOpened);
    }

    public GameObject GetCraftingPanel() => craftingPanel;

    public void OpenChest()
    {
        IsChestOpened = true;
        ToggleInventory();
    }

    public void CloseChest()
    {
        IsChestOpened = false;
        if (IsInventoryOpened)
        {
            ToggleInventory();
        }
    }

    public void OpenCraftingBuilding()
    {
        IsCraftingBuildingOpened = true;
        ToggleInventory();
    }

    private void ShowSelectionCraftingBuildingUI()
    {
        InventoryManager.Instance.OpenedCraftingBuilding.ReturnItemsToInventory();
        craftingUI.SetActive(false);
        selectionUI.SetActive(true);

        InventoryManager.Instance.OpenedCraftingBuilding.SetupRecipesSlots(recipesContainer);
    }

    public void ShowCraftCraftingBuildingUI()
    {
        selectionUI.SetActive(false);
        craftingUI.SetActive(true);
    }

    //public void Refresh()
    //{
    //    // очищаем старые
    //    foreach (Transform child in slotsParent)
    //        Destroy(child.gameObject);

    //    slotViews.Clear();

    //    foreach (var slotData in container.Slots)
    //    {
    //        var slot = Instantiate(slotPrefab, slotsParent);
    //        slot.Set(slotData.Item, slotData.Amount);
    //        slotViews.Add(slot);
    //    }
    //}
}
