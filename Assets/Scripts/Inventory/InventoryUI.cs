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
    [SerializeField] private Transform outputSlotObject;
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

    public bool IsInventoryOpened => InventoryPanel != null && InventoryPanel.activeSelf;
    public bool IsChestOpened { get; private set; }
    public bool IsCraftingBuildingOpened { get; private set; }

    public Transform InputSlotsContainer => inputSlotsContainer;
    public Transform OutputSlotObject => outputSlotObject;

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
        if (IsInventoryOpened)
        {
            Close();
            return;
        }

        Open();
    }

    public void Open()
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

    public void Close()
    {
        // Persist state when closing contextual UIs
        if (chestInventory != null && chestInventory.activeSelf)
        {
            InventoryManager.Instance?.SaveChestInventory();
        }

        if (buildingsUI != null && buildingsUI.activeSelf)
        {
            var mgr = InventoryManager.Instance;
            if (mgr != null && mgr.OpenedCraftingBuilding != null)
            {
                mgr.OpenedCraftingBuilding.SaveData();
            }
        }

        IsChestOpened = false;
        IsCraftingBuildingOpened = false;

        if (selectionUI != null) selectionUI.SetActive(false);
        if (craftingUI != null) craftingUI.SetActive(false);
        if (buildingsUI != null) buildingsUI.SetActive(false);
        if (chestInventory != null) chestInventory.SetActive(false);

        if (inventory != null) inventory.SetActive(false);
        if (craftingPanel != null) craftingPanel.SetActive(false);
        if (closeButton != null) closeButton.SetActive(false);
        if (InventoryPanel != null) InventoryPanel.SetActive(false);
    }

    private void OpenPlayerInventoryUI()
    {
        // Player inventory (no contextual building/chest UI)
        if (buildingsUI != null) buildingsUI.SetActive(false);
        if (chestInventory != null) chestInventory.SetActive(false);
        if (selectionUI != null) selectionUI.SetActive(false);
        if (craftingUI != null) craftingUI.SetActive(false);

        if (InventoryPanel != null) InventoryPanel.SetActive(true);
        if (inventory != null) inventory.SetActive(true);

        if (closeButton != null) closeButton.SetActive(true);
        if (craftingPanel != null) craftingPanel.SetActive(true);
    }

    private void OpenChestUI()
    {
        // Chest UI implies the main inventory UI is open
        if (InventoryPanel != null) InventoryPanel.SetActive(true);

        if (inventory != null) inventory.SetActive(true);
        if (closeButton != null) closeButton.SetActive(true);

        if (craftingPanel != null) craftingPanel.SetActive(false);
        if (craftingUI != null) craftingUI.SetActive(false);
        if (selectionUI != null) selectionUI.SetActive(false);

        if (buildingsUI != null) buildingsUI.SetActive(true);
        if (chestInventory != null) chestInventory.SetActive(true);
    }

    private void OpenCraftingBuildingUI()
    {
        // Crafting building UI implies the main inventory UI is open
        if (InventoryPanel != null) InventoryPanel.SetActive(true);

        if (inventory != null) inventory.SetActive(true);
        if (closeButton != null) closeButton.SetActive(true);
        if (craftingPanel != null) craftingPanel.SetActive(false);

        if (buildingsUI != null) buildingsUI.SetActive(true);

        var mgr = InventoryManager.Instance;
        var building = mgr != null ? mgr.OpenedCraftingBuilding : null;
        if (building == null)
        {
            if (craftingUI != null) craftingUI.SetActive(false);
            if (selectionUI != null) selectionUI.SetActive(false);
            return;
        }

        if (!building.IsCrafting)
        {
            // Only recipes list
            if (craftingUI != null) craftingUI.SetActive(false);
            if (selectionUI != null) selectionUI.SetActive(true);
            building.SetupRecipesSlots(recipeContainer);
        }
        else
        {
            // Crafting in progress / crafting view
            if (craftingUI != null) craftingUI.SetActive(true);
            if (selectionUI != null) selectionUI.SetActive(false);
            building.InizializeUICraftingSlots();
        }
    }

    public GameObject GetCraftingPanel() => craftingPanel;

    public void OpenChest()
    {
        IsChestOpened = true;
        IsCraftingBuildingOpened = false;
        Open();
    }

    public void OpenCraftingBuilding()
    {
        IsCraftingBuildingOpened = true;
        IsChestOpened = false;
        Open();
    }

    public void ChangeCraftAndSelectionCraftingBuilding()
    {
        if (!InventoryManager.Instance.OpenedCraftingBuilding.IsCrafting)
        {
            // ����� ���������� 
            InventoryManager.Instance.OpenedCraftingBuilding.InizializeUICraftingSlots();
        }
        else
        {
            // ���� �����
            InventoryManager.Instance.OpenedCraftingBuilding.SaveData();
            InventoryManager.Instance.OpenedCraftingBuilding.ReturnItemsToInventory();
            InventoryManager.Instance.OpenedCraftingBuilding.SetupRecipesSlots(recipeContainer);
        }

        selectionUI.SetActive(!selectionUI.activeSelf);
        craftingUI.SetActive(!craftingUI.activeSelf);
    }

    //public void Refresh()
    //{
    //    // ������� ������
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
