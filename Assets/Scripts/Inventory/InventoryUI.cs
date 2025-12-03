using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Buildings References")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject selectionUI;
    [SerializeField] private GameObject recipeContainer;
    [SerializeField] private GameObject craftingUI;
    [SerializeField] private GameObject buildingsUI;
    [SerializeField] private Transform inputSlotsContainer;
    [SerializeField] private SpecificItemSlot outputSlot;
    [SerializeField] private GameObject recipeItemSlotPrefab;
    [SerializeField] private GameObject specificItemSlotPrefab;
    [SerializeField] public Slider craftingProgress;


    [SerializeField] private Button backToSelectionButton;
    [SerializeField] private Button closeUIButtonCB;

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

    public Transform InputSlotsContainer => inputSlotsContainer;
    public SpecificItemSlot OutputSlot => outputSlot;

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
            OpenCraftingBuildingUI();
        }
        else if (IsChestOpened)
        {
            OpenChestUI();
        }
        else
        {
            OpenPlayerInventoryUI();
        }
    }

    private void OpenPlayerInventoryUI()
    {
        buildingsUI.SetActive(false);
        closeButton.SetActive(!IsInventoryOpened);
        craftingPanel.SetActive(!chestInventory.activeSelf);
        InventoryPanel.SetActive(!IsInventoryOpened);

        // последним, потому что на него ориентируется IsInventoryOpened
        inventory.SetActive(!IsInventoryOpened);
    }

    private void OpenChestUI()
    {
        inventory.SetActive(!IsInventoryOpened);
        craftingUI.SetActive(false);
        selectionUI.SetActive(false);

        if (chestInventory.activeSelf)
        {
            InventoryManager.Instance.SaveChestInventory();
            IsChestOpened = false;
        }

        buildingsUI.SetActive(IsChestOpened);
        chestInventory.SetActive(IsChestOpened);
    }

    private void OpenCraftingBuildingUI()
    {
        inventory.SetActive(!IsInventoryOpened);

        if (buildingsUI.activeSelf)
        {
            IsCraftingBuildingOpened = false;
            InventoryManager.Instance.OpenedCraftingBuilding.SaveData();
        }

        buildingsUI.SetActive(IsCraftingBuildingOpened);
        if (!InventoryManager.Instance.OpenedCraftingBuilding.IsCrafting)
        {
            // суйчас выбор крафта
            craftingUI.SetActive(!IsCraftingBuildingOpened && buildingsUI.activeSelf);
            selectionUI.SetActive(IsCraftingBuildingOpened && buildingsUI.activeSelf);
            InventoryManager.Instance.OpenedCraftingBuilding.SetupRecipesSlots(recipeContainer);
        }
        else
        {
            // сейчас что-то крафтиться
            craftingUI.SetActive(IsCraftingBuildingOpened && buildingsUI.activeSelf);
            selectionUI.SetActive(!IsCraftingBuildingOpened && buildingsUI.activeSelf);
            if (IsCraftingBuildingOpened) 
                InventoryManager.Instance.OpenedCraftingBuilding.InizializeUICraftingSlots();
        }
    }

    public GameObject GetCraftingPanel() => craftingPanel;

    public void OpenChest()
    {
        IsChestOpened = true;
        ToggleInventory();
    }

    public void OpenCraftingBuilding()
    {
        IsCraftingBuildingOpened = true;
        ToggleInventory();
    }

    public void ChangeCraftAndSelectionCraftingBuilding()
    {
        if (!InventoryManager.Instance.OpenedCraftingBuilding.IsCrafting)
        {
            // стало крафтиться 
            InventoryManager.Instance.OpenedCraftingBuilding.InizializeUICraftingSlots();
        }
        else
        {
            // стал выбор
            InventoryManager.Instance.OpenedCraftingBuilding.SaveData();
            InventoryManager.Instance.OpenedCraftingBuilding.ReturnItemsToInventory();
            InventoryManager.Instance.OpenedCraftingBuilding.SetupRecipesSlots(recipeContainer);
        }

        selectionUI.SetActive(!selectionUI.activeSelf);
        craftingUI.SetActive(!craftingUI.activeSelf);
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
