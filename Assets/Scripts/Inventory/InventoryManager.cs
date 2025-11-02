using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public InventoryUI ui;
    [SerializeField] private GameObject inventory, quickSlots, chestInventory;

    private InventoryContainer playerContainer;
    private QuickSlotsInventoryContainer playerQuickContainer;

    public bool IsChestOpened => ui.IsChestOpened;

    public Chest OpenedChest { get; private set; }

    public CraftingBuilding OpenedCraftingBuilding { get; private set; }

    private void Awake()
    {
        Instance = this;

        InitializeContainers();
        InitializeUI();

        // ui ���������� ������� ��� �������������, ��������� ���
        ui.ToggleInventory(); 
    }

    private void InitializeContainers()
    {
        playerContainer = new InventoryContainer(inventory);
        playerQuickContainer = new QuickSlotsInventoryContainer(quickSlots);

        //FillSlotList(chestInventory, chestInventorySlots);
    }

    private void InitializeUI()
    {
        if (ui != null)
        {
            ui.Initialize(playerContainer);
        }


        //if (chestInventoryUI != null)
        //{
        //    chestInventoryUI.Hide();
        //}
    }

    private void OnEnable()
    {
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnInventoryToggle += HandleInventoryToggle;
            InputHandler.Instance.OnScrollUp += HandleScrollUp;
            InputHandler.Instance.OnScrollDown += HandleScrollDown;
            InputHandler.Instance.OnQuickSlotSelect += HandleQuickSlotSelect;
            InputHandler.Instance.OnBuildingPlace += HandleBuildingPlace;
        }
    }

    private void OnDisable()
    {
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnInventoryToggle -= HandleInventoryToggle;
            InputHandler.Instance.OnScrollUp -= HandleScrollUp;
            InputHandler.Instance.OnScrollDown -= HandleScrollDown;
            InputHandler.Instance.OnQuickSlotSelect -= HandleQuickSlotSelect;
            InputHandler.Instance.OnBuildingPlace -= HandleBuildingPlace;
        }
    }

    private void Update()
    {
        // Проверка на пустой активный слот при размещении здания
        if (BuildingsGrid.Instance.IsPlacingBuilding && 
            playerQuickContainer.activeSlot != null && 
            playerQuickContainer.activeSlot.Amount == 0)
        {
            BuildingsGrid.Instance.StopPlacingBuilding();
        }
    }

    private void HandleInventoryToggle()
    {
        ui.ToggleInventory();
    }

    private void HandleScrollUp()
    {
        if (!ui.IsInventoryOpened)
        {
            playerQuickContainer.ScrollUp();
        }
    }

    private void HandleScrollDown()
    {
        if (!ui.IsInventoryOpened)
        {
            playerQuickContainer.ScrollDown();
        }
    }

    private void HandleQuickSlotSelect(int slotNumber)
    {
        if (!ui.IsInventoryOpened)
        {
            playerQuickContainer.ChangeActiveSlotTo(slotNumber - 1);
        }
    }

    private void HandleBuildingPlace()
    {
        if (ui.IsInventoryOpened) return;

        if (BuildingsGrid.Instance.IsPlacingBuilding)
        {
            BuildingsGrid.Instance.StopPlacingBuilding();
        }
        else if (playerQuickContainer.activeSlot != null && 
            playerQuickContainer.activeSlot.Item != null && 
            playerQuickContainer.activeSlot.Item.itemType == ItemType.Building)
        {
            BuildingsGrid.Instance.StartPlacingBuilding(playerQuickContainer.activeSlot.Item.itemPrefab.GetComponent<Building>());
        }
    }

    public void AddOneItemToInventory(ItemScriptableObject item)
    {
        int amount = 1;
        amount = playerQuickContainer.AddItems(item, amount);
        amount = playerContainer.AddItems(item, amount);
    }

    public int AddItemsToInventory(ItemScriptableObject item, int amount)
    {
        if (amount <= 0) return 0;

        amount = playerQuickContainer.AddItems(item, amount);
        amount = playerContainer.AddItems(item, amount);

        return amount; // �������, ���� �� ������� �����
    }

    public int AddToOpenedChest(ItemScriptableObject item, int amount)
        => OpenedChest.AddItems(item, amount);

    public int CountItem(ItemScriptableObject item)
    {
        int count = 0;

        foreach (var slot in playerContainer.Slots)
        {
            if (slot.Item == item)
                count += slot.Amount;
        }

        foreach (var slot in playerQuickContainer.Slots)
        {
            if (slot.Item == item)
                count += slot.Amount;
        }

        return count;
    }

    public void RemoveItems(ItemScriptableObject item, int amount)
    {
        foreach (var slot in playerContainer.Slots)
        {
            if (amount <= 0) break;

            if (slot.Item == item)
            {
                int removeAmount = Mathf.Min(slot.Amount, amount);
                slot.RemoveAmount(removeAmount);
                amount -= removeAmount;
            }
        }

        foreach (var slot in playerQuickContainer.Slots)
        {
            if (amount <= 0) break;

            if (slot.Item == item)
            {
                int removeAmount = Mathf.Min(slot.Amount, amount);
                slot.RemoveAmount(removeAmount);
                amount -= removeAmount;
            }
        }
    }

    public void RemoveUsedItemFromActiveSlot()
    {
        playerQuickContainer.RemoveUsedItemFromActiveSlot();
    }

    public void OpenChest(Chest chest)
    {
        OpenedChest = chest;
        OpenedChest.InizializeUISlotsFromParentObj(chestInventory);
        ui.OpenChest();
    }

    public List<InventorySlot> GetChestInventorySlots()
    {
        return new List<InventorySlot>();
        //return OpenedChest.inventoryContainer.Slots;
    }

    public void OpenCraftingBuilding(CraftingBuilding building)
    {
        OpenedCraftingBuilding = building;
    }

    public void CloseInventoryFromButton()
    {
        ui.ToggleInventory();
    }
}

