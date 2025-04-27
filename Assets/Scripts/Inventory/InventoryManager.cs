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

    [SerializeField] private GameObject inventory, UIPanel, chestInventory, quickSlots;
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

        inventory.SetActive(false);
        chestInventory.SetActive(false);
        UIPanel.SetActive(false);

        InitializeSlots();
    }

    private void InitializeSlots()
    {
        for (int i = 0; i < inventory.transform.childCount; i++)
        {
            if (inventory.transform.GetChild(i).GetComponent<InventorySlot>() != null)
            {
                inventorySlots.Add(inventory.transform.GetChild(i).GetComponent<InventorySlot>());
            }
        }
        for (int i = 0; i < chestInventory.transform.childCount; i++)
        {
            if (chestInventory.transform.GetChild(i).GetComponent<InventorySlot>() != null)
            {
                chestInventorySlots.Add(chestInventory.transform.GetChild(i).GetComponent<InventorySlot>());
            }
        }
        for (int i = 0; i < quickSlots.transform.childCount; i++)
        {
            if (quickSlots.transform.GetChild(i).GetComponent<InventorySlot>() != null)
            {
                quickInventorySlots.Add(quickSlots.transform.GetChild(i).GetComponent<InventorySlot>());
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            CloseOpenInventory();

        //if (Input.GetMouseButtonDown(0))
        //    TryToGetItem();
    }

    private void TryToGetItem()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, reachDistance))
        {
            Item item = hit.collider.gameObject.GetComponent<Item>();
            if (item != null)
            {
                AddItem(item.item, item.amount);
                Destroy(item.gameObject);
            }
        }
    }

    private void CloseOpenInventory()
    {
        isOpened = !isOpened;
        inventory.SetActive(isOpened);
        UIPanel.SetActive(isOpened);
        //Cursor.lockState = isOpened ? CursorLockMode.None : CursorLockMode.Locked;
        //Cursor.visible = isOpened;

        quickSlots.SetActive(IsChestOpened ? !isOpened : true);
        inventory.transform.localPosition = new Vector3(0, IsChestOpened ? -250 : 0, 0);
        if (chestInventory.activeSelf)
        {
            OpenedChest.SetSlots(chestInventorySlots.ToArray());
        }
        chestInventory.SetActive(IsChestOpened ? isOpened : false);
        
        if (OpenedChest)
            OpenedChest.SetIsOpen(IsChestOpened);

        IsChestOpened = false;
    }

    private void AddItem(ItemScriptableObject item, int amount)
    {
        foreach (InventorySlot slot in quickInventorySlots)
        {
            if (slot.isEmpty)
            {
                slot.PlaceItem(item, amount);
                return;
            }

            if (slot.Item == item && slot.Amount < item.maximumAmount)
            {
                if (slot.Amount + amount <= item.maximumAmount)
                    slot.AddAmount(amount);
                else
                {
                    amount -= item.maximumAmount - slot.Amount;
                    slot.AddAmount(item.maximumAmount - slot.Amount);
                    AddItem(item, amount);
                }
                return;
            }
        }
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.isEmpty)
            {
                slot.PlaceItem(item, amount);
                return;
            }

            if (slot.Item == item && slot.Amount < item.maximumAmount)
            {
                if (slot.Amount + amount <= item.maximumAmount)
                    slot.AddAmount(amount);
                else
                {
                    amount -= item.maximumAmount - slot.Amount;
                    slot.AddAmount(item.maximumAmount - slot.Amount);
                    AddItem(item, amount);
                }
                return;
            }
        }
    }

    public int TryAddItem(ItemScriptableObject item, int amount)
    {
        foreach (InventorySlot slot in inventorySlots)
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

    public void TryAddOneItem(ItemScriptableObject item)
    {
        AddItem(item, 1);
    }

    public int TryAddItemToChest(ItemScriptableObject item, int amount)
    {
        if (!IsChestOpened)
            throw new Exception("Сундук не открыт");

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

    public void GoToTheChest(Chest chest)
    {
        OpenedChest = chest;
        for (int i = 0; i < chest.Slots.Length; i++)
        {
            chestInventorySlots[i].PlaceItem(chest.Slots[i].Item, chest.Slots[i].Amount);
        }
        IsChestOpened = true;
    }

    public void OpenChest(Chest chest)
    {
        OpenedChest = chest;
        for (int i = 0; i < chest.Slots.Length; i++)
        {
            chestInventorySlots[i].PlaceItem(chest.Slots[i].Item, chest.Slots[i].Amount);
        }
        IsChestOpened = true;
        if (!isOpened)
        {
            CloseOpenInventory();
        }
    }

    public void GoAwayFromTheChest()
    {
        IsChestOpened = false;
    }

    public List<InventorySlot> GetChestInventorySlots()
    {
        return chestInventorySlots;
    }
}
