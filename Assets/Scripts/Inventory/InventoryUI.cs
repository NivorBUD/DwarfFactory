using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject UIPanel;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject chestInventory;
    [SerializeField] private GameObject quickSlots;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private GameObject craftingPanel;

    public bool IsInventoryOpened => UIPanel.activeSelf;
    public bool IsChestOpened { get; private set; }

    private InventoryContainer container;

    public void OpenChest()
    {
        IsChestOpened = true;
        ToggleInventory();
    }

    public void CloseChest()
    {
        IsChestOpened = false;
        if (IsInventoryOpened)
        {
            ToggleInventory();
        }
    }

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
        inventory.SetActive(!IsInventoryOpened);
        closeButton.SetActive(!IsInventoryOpened);
        craftingPanel.SetActive(!IsInventoryOpened);

        // зависит от открытия сундука
        //chestInventory.SetActive(IsChestOpened ? !IsInventoryOpened : false);
        //quickSlots.SetActive(IsChestOpened ? !IsInventoryOpened : true);
        //inventory.transform.localPosition = new Vector3(0, IsChestOpened ? -250 : 0, 0);
        

        IsChestOpened = false;
        // последним, потому что на него ориентируется IsInventoryOpened
        UIPanel.SetActive(!IsInventoryOpened);
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
