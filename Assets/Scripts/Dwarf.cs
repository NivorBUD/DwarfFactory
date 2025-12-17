using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dwarf : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private Transform inventorySlotsParent;
    private InventoryContainer inventoryContainer;

    [Header("Equipment Slots")]
    private AllowedTypeSlot HelmetSlot;
    private AllowedTypeSlot ChestSlot;
    private AllowedTypeSlot BootsSlot;
    private AllowedTypeSlot WeaponSlot;

    public InventoryContainer Inventory => inventoryContainer;

    private void Awake()
    {
        inventoryContainer = new InventoryContainer(inventorySlotsParent.gameObject);

        HelmetSlot = new();
        HelmetSlot.SetAllowedType(ItemType.Armor);

        ChestSlot = new();
        ChestSlot.SetAllowedType(ItemType.Armor);

        BootsSlot = new();
        BootsSlot.SetAllowedType(ItemType.Armor);

        WeaponSlot = new();
        WeaponSlot.SetAllowedType(ItemType.Weapon);
    }

    private void FixedUpdate()
    {
        if (InventoryManager.Instance.OpenedDwarf == this && InventoryManager.Instance.ui.IsDwarfOpened)
        {
            SaveData(new List<InventorySlot>());
        }
    }

    public bool EquipItem(ItemScriptableObject item)
    {
        if (item.itemType == ItemType.Weapon)
        {
            WeaponSlot.Set(item, 1);
            return true;
        }

        if (item.itemType == ItemType.Armor)
        {
            // здесь можно расширить подтипы
            if (HelmetSlot.IsAllowed(item) && HelmetSlot.IsEmpty)
            {
                HelmetSlot.Set(item, 1);
                return true;
            }
            if (ChestSlot.IsAllowed(item) && ChestSlot.IsEmpty)
            {
                ChestSlot.Set(item, 1);
                return true;
            }
            if (BootsSlot.IsAllowed(item) && BootsSlot.IsEmpty)
            {
                BootsSlot.Set(item, 1);
                return true;
            }
        }

        return false;
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                Interaction();
            }
            
        }
    }

    public void UnequipAll()
    {
        HelmetSlot.Clear();
        ChestSlot.Clear();
        BootsSlot.Clear();
        WeaponSlot.Clear();
    }

    public void InizializeUISlotsFromSlotsList(List<InventorySlot> dwarfUISlots)
    {
        bool isSlotsSet = inventoryContainer.Slots.Count == dwarfUISlots.Count;
        List<InventorySlot> slots = new();
        for (int i = 0; i < dwarfUISlots.Count; i++)
        {
            if (isSlotsSet)
            {
                dwarfUISlots[i].Set(inventoryContainer.Slots[i].Item, inventoryContainer.Slots[i].Amount);
            }
            else
            {
                dwarfUISlots[i].Clear();
                slots.Add(dwarfUISlots[i].Copy());
            }
        }
        if (!isSlotsSet)
        {
            inventoryContainer.SetNewSlots(slots);
        }

        InventoryManager.Instance.ui.DwarfHelmetSlot.Set(HelmetSlot.Item);
        InventoryManager.Instance.ui.DwarfChestSlot.Set(ChestSlot.Item);
        InventoryManager.Instance.ui.DwarfBootsSlot.Set(BootsSlot.Item);
        InventoryManager.Instance.ui.DwarfWeaponSlot.Set(WeaponSlot.Item);
    }

    public void SaveData(List<InventorySlot> newSlots)
    {
        if (newSlots.Count > 0)
        {
            for (int i = 0; i < inventoryContainer.Slots.Count; i++)
            {
                inventoryContainer.Slots[i].Set(newSlots[i].Item, newSlots[i].Amount);
            }
        }

        HelmetSlot.Set(InventoryManager.Instance.ui.DwarfHelmetSlot.Item);
        ChestSlot.Set(InventoryManager.Instance.ui.DwarfChestSlot.Item);
        BootsSlot.Set(InventoryManager.Instance.ui.DwarfBootsSlot.Item);
        WeaponSlot.Set(InventoryManager.Instance.ui.DwarfWeaponSlot.Item);
    }

    public int AddToInventory(ItemScriptableObject item, int amount)
        => inventoryContainer.AddItems(item, amount);

    public bool RemoveFromInventory(ItemScriptableObject item, int amount)
        => inventoryContainer.TryRemoveItem(item, amount);

    public void Interaction()
    {
        InventoryManager.Instance.OpenDwarf(this);
    }
}