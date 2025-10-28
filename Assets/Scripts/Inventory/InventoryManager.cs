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
    public bool IsChestOpened { get; private set; }
    public Chest OpenedChest { get; private set; }
    public CraftingBuilding OpenedCraftingBuilding { get; private set; }
    [SerializeField] private GameObject closeButton;
    [SerializeField] private GameObject craftingPanel;

    [SerializeField] private GameObject inventory, UIPanel, chestInventory, quickSlots;
    public InventoryUI ui;
    private List<InventorySlot> inventorySlots, chestInventorySlots, quickInventorySlots;

    private bool isOpened;
    private Camera mainCamera;
    private float reachDistance = 20;

    private void Awake()
    {
        inventorySlots = new();
        chestInventorySlots = new();
        quickInventorySlots = new();
        Instance = this;
        mainCamera = Camera.main;

        InitializeSlots();
    }

    private void InitializeSlots()
    {
        FillSlotList(inventory, inventorySlots);
        FillSlotList(chestInventory, chestInventorySlots);
        FillSlotList(quickSlots, quickInventorySlots);
    }

    private void FillSlotList(GameObject parent, List<InventorySlot> slotList)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if (parent.transform.GetChild(i).TryGetComponent(out InventorySlot slot))
            {
                slotList.Add(slot);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ui.ToggleInventory();
        }
            

        //if (Input.GetMouseButtonDown(0))
        //    TryToGetItem();
    }

    private void CloseOpenInventory()
    {  
        isOpened = !isOpened;
        inventory.SetActive(isOpened);
        UIPanel.SetActive(isOpened);
        closeButton.SetActive(isOpened);
        craftingPanel.SetActive(isOpened);
        //Cursor.lockState = isOpened ? CursorLockMode.None : CursorLockMode.Locked;
        //Cursor.visible = isOpened;

        quickSlots.SetActive(IsChestOpened ? !isOpened : true);
        //inventory.transform.localPosition = new Vector3(0, IsChestOpened ? -250 : 0, 0);
        if (chestInventory.activeSelf)
        {
            OpenedChest.SetSlots(chestInventorySlots.ToArray());
        }
        chestInventory.SetActive(IsChestOpened ? isOpened : false);
        
        if (OpenedChest)
            OpenedChest.SetIsOpen(IsChestOpened);

        IsChestOpened = false;
    }

    public void AddOneItem(ItemScriptableObject item)
    {
        int amount = 1;
        TryAddToSlots(quickInventorySlots, item, ref amount);
        TryAddToSlots(inventorySlots, item, ref amount);
    }

    public int AddItem(ItemScriptableObject item, int amount)
    {
        if (amount <= 0) return 0;

        TryAddToSlots(quickInventorySlots, item, ref amount);
        TryAddToSlots(inventorySlots, item, ref amount);

        return amount; // остаток, если не хватило места
    }

    private void TryAddToSlots(List<InventorySlot> slots, ItemScriptableObject item, ref int amount)
    {
        // ƒобавл€ем в существующие стаки
        foreach (var slot in slots)
        {
            if (amount <= 0) return;

            if (!slot.isEmpty && slot.Item == item && slot.Amount < item.maximumAmount)
            {
                int space = item.maximumAmount - slot.Amount;
                int addAmount = Mathf.Min(amount, space);
                slot.AddAmount(addAmount);
                amount -= addAmount;
            }
        }

        // ƒобавл€ем в пустые слоты
        foreach (var slot in slots)
        {
            if (amount <= 0) return;

            if (slot.isEmpty)
            {
                int placeAmount = Mathf.Min(amount, item.maximumAmount);
                slot.PlaceItem(item, placeAmount);
                amount -= placeAmount;
            }
        }
    }

    public int TryAddToChest(ItemScriptableObject item, int amount)
    {
        foreach (InventorySlot slot in chestInventorySlots)
        {
            if (amount == 0)
            {
                return 0;
            }

            if (slot.isEmpty)
            {
                slot.PlaceItem(item, amount);
                return 0;
            }

            if (slot.Item == item && slot.Amount < item.maximumAmount)
            {
                if (slot.Amount + amount <= item.maximumAmount)
                {
                    slot.AddAmount(amount);
                    amount = 0;
                }
                else
                {
                    amount -= item.maximumAmount - slot.Amount;
                    slot.AddAmount(item.maximumAmount - slot.Amount);
                }
            }
        }
        return amount;
    }

    public int CountItem(ItemScriptableObject item)
    {
        int count = 0;

        foreach (var slot in inventorySlots)
        {
            if (slot.Item == item)
                count += slot.Amount;
        }

        foreach (var slot in quickInventorySlots)
        {
            if (slot.Item == item)
                count += slot.Amount;
        }

        return count;
    }

    public void RemoveItems(ItemScriptableObject item, int amount)
    {
        foreach (var slot in inventorySlots)
        {
            if (amount <= 0) break;

            if (slot.Item == item)
            {
                int removeAmount = Mathf.Min(slot.Amount, amount);
                slot.RemoveAmount(removeAmount);
                amount -= removeAmount;
            }
        }

        foreach (var slot in quickInventorySlots)
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

    public void SetChestInventory(Chest chest)
    {
        OpenedChest = chest;
        for (int i = 0; i < chest.Slots.Length; i++)
        {
            chestInventorySlots[i].PlaceItem(chest.Slots[i].Item, chest.Slots[i].Amount);
        }
        ui.OpenChest();
        IsChestOpened = true;
    }

    public void OpenChest(Chest chest)
    {
        SetChestInventory(chest);
        if (!isOpened)
        {
            ui.ToggleInventory();
        }
    }

    public void OpenCraftingBuilding(CraftingBuilding building)
    {
        OpenedCraftingBuilding = building;
    }

    public void GoAwayFromTheChest()
    {
        ui.CloseChest();
    }

    public List<InventorySlot> GetChestInventorySlots()
    {
        return chestInventorySlots;
    }
    
    public void CloseInventoryFromButton()
    {
        if (isOpened)
        {
            ui.ToggleInventory();
        }
    }    
    
    //private void TryToGetItem()
    //{
    //    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit, reachDistance))
    //    {
    //        Item item = hit.collider.gameObject.GetComponent<Item>();
    //        if (item != null)
    //        {
    //            AddItem(item.item, item.amount);
    //            Destroy(item.gameObject);
    //        }
    //    }
    //}
}

