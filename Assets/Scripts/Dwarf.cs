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

    [Header("Visual Equipment")]
    [SerializeField] private GameObject swordVisual;
    [SerializeField] private GameObject leatherBodyVisual;

    public InventoryContainer Inventory => inventoryContainer;

    private void Awake()
    {
        inventoryContainer = new InventoryContainer(inventorySlotsParent.gameObject);

        HelmetSlot = new();
        HelmetSlot.SetAllowedType(ItemType.Helmet);

        ChestSlot = new();
        ChestSlot.SetAllowedType(ItemType.Chestplate);

        BootsSlot = new();
        BootsSlot.SetAllowedType(ItemType.Boots);

        WeaponSlot = new();
        WeaponSlot.SetAllowedType(ItemType.Weapon);

        // Автоматически находим визуальные элементы если не назначены
        if (swordVisual == null)
        {
            Transform sword = transform.Find("RightHand_0/Sword");
            if (sword != null)
                swordVisual = sword.gameObject;
        }

        if (leatherBodyVisual == null)
        {
            Transform leather = transform.Find("Body_0/LeatherBody");
            if (leather != null)
                leatherBodyVisual = leather.gameObject;
        }

        // Обновляем визуализацию при старте
        UpdateEquipmentVisuals();
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

        if (item.itemType == ItemType.Helmet && HelmetSlot.IsEmpty)
        {
            HelmetSlot.Set(item, 1);
            return true;
        }
        if (item.itemType == ItemType.Chestplate && ChestSlot.IsEmpty)
        {
            ChestSlot.Set(item, 1);
            return true;
        }
        if (item.itemType == ItemType.Boots && BootsSlot.IsEmpty)
        {
            BootsSlot.Set(item, 1);
            return true;
        }

        return false;
    }

    // Удалено открытие по клику мыши - теперь используется кнопка G через InputHandler

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

        // Обновляем визуализацию после сохранения
        UpdateEquipmentVisuals();
    }

    public int AddToInventory(ItemScriptableObject item, int amount)
        => inventoryContainer.AddItems(item, amount);

    public bool RemoveFromInventory(ItemScriptableObject item, int amount)
        => inventoryContainer.TryRemoveItem(item, amount);

    public void Interaction()
    {
        InventoryManager.Instance.OpenDwarf(this);
    }

    private void UpdateEquipmentVisuals()
    {
        // Показываем/скрываем меч
        if (swordVisual != null)
        {
            swordVisual.SetActive(!WeaponSlot.IsEmpty);
        }

        // LeatherBody показываем только когда надета броня
        if (leatherBodyVisual != null)
        {
            leatherBodyVisual.SetActive(!ChestSlot.IsEmpty);
        }
    }

    // Публичный метод для принудительного обновления визуализации
    public void RefreshVisuals()
    {
        UpdateEquipmentVisuals();
    }
}
