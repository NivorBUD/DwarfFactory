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
    private List<InventorySlot> chestSlots;

    private InventoryContainer playerContainer;
    private QuickSlotsInventoryContainer playerQuickContainer;

    public bool IsChestOpened => ui.IsChestOpened;

    public Chest OpenedChest { get; private set; }

    public CraftingBuilding OpenedCraftingBuilding { get; private set; }
    public Dwarf OpenedDwarf { get; private set; }

    private void Awake()
    {
        Instance = this;

        InitializeContainers();
        InitializeUI();

        // ui ���������� ������� ��� �������������, ��������� ���
        ui.Close();
    }

    private void InitializeContainers()
    {
        playerContainer = new InventoryContainer(inventory);
        playerQuickContainer = new QuickSlotsInventoryContainer(quickSlots);

        chestSlots = new();
        for (int i = 0; i < chestInventory.transform.childCount; i++)
        {
            if (chestInventory.transform.GetChild(i).TryGetComponent(out InventorySlot slot))
            {
                chestSlots.Add(slot);
            }
        }

        //FillSlotList(chestInventory, chestInventorySlots);
    }

    private void InitializeUI()
    {
        if (ui != null)
        {
            ui.Initialize(playerContainer);
        }


        if (chestInventory != null)
        {
            chestInventory.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ui.ToggleInventory();
        }


        // ��� ����� ��������� ����� ��� ������� ������
        if (ui.IsInventoryOpened) return;

        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        if (mouseWheel > 0.1)
            playerQuickContainer.ScrollUp();

        if (mouseWheel < -0.1)
            playerQuickContainer.ScrollDown();

        playerQuickContainer.CheckNums();

        if (Input.GetKeyDown(KeyCode.F))
        {
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

        if (BuildingsGrid.Instance.IsPlacingBuilding && playerQuickContainer.activeSlot.Amount == 0)
        {
            BuildingsGrid.Instance.StopPlacingBuilding();
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
        OpenedChest.InizializeUISlotsFromSlotsList(chestSlots);
        ui.OpenChest();
    }

    public void SaveChestInventory()
    {
        if (OpenedChest != null)
        {
            OpenedChest.SaveData(chestSlots);
        }
    }

    public List<InventorySlot> GetChestInventorySlots()
    {
        return new List<InventorySlot>();
        //return OpenedChest.inventoryContainer.Slots;
    }

    public void OpenCraftingBuilding(CraftingBuilding building)
    {
        OpenedCraftingBuilding = building;
        ui.OpenCraftingBuilding();
    }

    public void OpenDwarf(Dwarf dwarf)
    {
        OpenedDwarf = dwarf;
        ui.OpenDwarf();
    }

    public void CloseInventoryFromButton()
    {
        ui.Close();
    }

    /// <summary>
    /// Generic close handler for UI buttons. Assign this to any CloseButton (OnClick) and pass the menu root GameObject.
    /// </summary>
    public void ClosePanel(GameObject panelRoot)
    {
        if (panelRoot == null) return;
        panelRoot.SetActive(false);
    }

    public int CalculateMaxCrafts(CraftingRecipe recipe)
    {
        int maxCrafts = -1;
        foreach (var ingredient in recipe.ingredients)
        {
            int playerAmount = InventoryManager.Instance.CountItem(ingredient.item);
            int craftsForThisItem = playerAmount / ingredient.amount;
            if (craftsForThisItem < maxCrafts || maxCrafts == -1)
                maxCrafts = craftsForThisItem;
        }
        return maxCrafts;
    }
}
