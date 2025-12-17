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

    [Header("UI Dwarf References")]
    [SerializeField] private GameObject DwarfUI;
    [SerializeField] private GameObject DwarfInventory;
    [SerializeField] private AllowedTypeSlot dwarfHelmetSlot;
    [SerializeField] private AllowedTypeSlot dwarfChestSlot;
    [SerializeField] private AllowedTypeSlot dwarfBootsSlot;
    [SerializeField] private AllowedTypeSlot dwarfWeaponSlot;

    [Header("UI Player References")]
    [SerializeField] private GameObject InventoryPanel;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject chestInventory;
    [SerializeField] private GameObject quickSlots;
    [SerializeField] private GameObject craftingPanel;

    private bool IsUIOpen;

    public bool IsInventoryOpened => IsUIOpen;
    public bool IsChestOpened { get; private set; }
    public bool IsCraftingBuildingOpened { get; private set; }
    public bool IsDwarfOpened { get; private set; }

    public Transform BuildingInputSlotsContainer => inputSlotsContainer;
    public Transform BuildingOutputSlotObject => outputSlotObject;
    public AllowedTypeSlot DwarfHelmetSlot => dwarfHelmetSlot;
    public AllowedTypeSlot DwarfChestSlot => dwarfChestSlot;
    public AllowedTypeSlot DwarfBootsSlot => dwarfBootsSlot;
    public AllowedTypeSlot DwarfWeaponSlot => dwarfWeaponSlot;


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
        IsUIOpen = true;
        if (IsCraftingBuildingOpened)
        {
            OpenCraftingBuildingUI();
        }
        else if (IsChestOpened)
        {
            OpenChestUI();
        }
        else if (IsDwarfOpened)
        {
            OpenDwarfUI();
        }
        else
        {
            OpenPlayerInventoryUI();
        }
    }

    public void Close()
    {
        IsUIOpen = false;

        if (chestInventory != null && chestInventory.activeSelf)
        {
            InventoryManager.Instance?.SaveChestInventory();
        }

        if (buildingsUI != null && buildingsUI.activeSelf)
        {
            if (InventoryManager.Instance.OpenedCraftingBuilding != null)
            {
                InventoryManager.Instance.OpenedCraftingBuilding.SaveData();
            }
        }

        IsChestOpened = false;
        IsCraftingBuildingOpened = false;
        IsDwarfOpened = false;

        DisActiveElements(DwarfInventory, DwarfUI, selectionUI, craftingUI, buildingsUI, chestInventory, inventory, craftingPanel, InventoryPanel);
    }

    private void OpenPlayerInventoryUI()
    {
        ActivateElements(InventoryPanel, inventory, craftingPanel);;
    }

    private void OpenChestUI()
    {
        ActivateElements(buildingsUI, inventory, chestInventory);
    }

    private void OpenCraftingBuildingUI()
    {
        ActivateElements(inventory, buildingsUI);

        var building = InventoryManager.Instance.OpenedCraftingBuilding;
        if (building == null)
        {
            Close();
            return;
        }

        if (!building.IsCrafting)
        {
            // Only recipes list
            craftingUI.SetActive(false);
            selectionUI.SetActive(true);
            building.SetupRecipesSlots(recipeContainer);
        }
        else
        {
            // Crafting in progress / crafting view
            craftingUI.SetActive(true);
            selectionUI.SetActive(false);
            building.InizializeUICraftingSlots();
        }
    }

    public GameObject GetCraftingPanel() => craftingPanel;

    public void OpenChest()
    {
        IsChestOpened = true;
        Open();
    }

    public void OpenCraftingBuilding()
    {
        IsCraftingBuildingOpened = true;
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
            InventoryManager.Instance.OpenedCraftingBuilding.ReturnItemsToPlayerInventory();
            InventoryManager.Instance.OpenedCraftingBuilding.SetupRecipesSlots(recipeContainer);
        }

        selectionUI.SetActive(!selectionUI.activeSelf);
        craftingUI.SetActive(!craftingUI.activeSelf);
    }

    public void OpenDwarf()
    {
        IsDwarfOpened = true;
        ToggleInventory();
    }

    public void OpenDwarfUI()
    {
        ActivateElements(inventory, DwarfUI, DwarfInventory, buildingsUI );
    }

    private void ActivateElements(params GameObject[] gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(true);
            } 
        }
    }

    private void DisActiveElements(params GameObject[] gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject != null) gameObject.SetActive(false);
        }
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
