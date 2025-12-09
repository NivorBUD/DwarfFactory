using UnityEngine;

public class Dwarf : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private Transform inventorySlotsParent;
    private InventoryContainer inventoryContainer;

    [Header("Equipment Slots")]
    [SerializeField] private AllowedTypeSlot helmetSlot;
    [SerializeField] private AllowedTypeSlot chestSlot;
    [SerializeField] private AllowedTypeSlot bootsSlot;
    [SerializeField] private AllowedTypeSlot weaponSlot;

    public InventoryContainer Inventory => inventoryContainer;

    private void Awake()
    {
        inventoryContainer = new InventoryContainer(inventorySlotsParent.gameObject);
    }

    public bool EquipItem(ItemScriptableObject item)
    {
        if (item.itemType == ItemType.Weapon)
        {
            weaponSlot.Set(item, 1);
            return true;
        }

        if (item.itemType == ItemType.Armor)
        {
            // здесь можно расширить подтипы
            if (helmetSlot.IsAllowed(item) && helmetSlot.IsEmpty)
            {
                helmetSlot.Set(item, 1);
                return true;
            }
            if (chestSlot.IsAllowed(item) && chestSlot.IsEmpty)
            {
                chestSlot.Set(item, 1);
                return true;
            }
            if (bootsSlot.IsAllowed(item) && bootsSlot.IsEmpty)
            {
                bootsSlot.Set(item, 1);
                return true;
            }
        }

        return false;
    }

    public void UnequipAll()
    {
        helmetSlot.Clear();
        chestSlot.Clear();
        bootsSlot.Clear();
        weaponSlot.Clear();
    }

    public int AddToInventory(ItemScriptableObject item, int amount)
        => inventoryContainer.AddItems(item, amount);

    public bool RemoveFromInventory(ItemScriptableObject item, int amount)
        => inventoryContainer.TryRemoveItem(item, amount);
}