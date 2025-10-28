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

    public bool IsOpened => UIPanel.activeSelf;
    public bool IsChestOpened { get; private set; }

    private void Awake()
    {
        ToggleInventory();
    }

    public void ToggleInventory()
    {   
        UIPanel.SetActive(!IsOpened);
        inventory.SetActive(!IsOpened);
        quickSlots.SetActive(!IsOpened);
        chestInventory.SetActive(!IsOpened);
        closeButton.SetActive(!IsOpened);
        craftingPanel.SetActive(!IsOpened);

        inventory.SetActive(IsOpened);
        UIPanel.SetActive(IsOpened);
        closeButton.SetActive(IsOpened);
        craftingPanel.SetActive(IsOpened);
        //Cursor.lockState = isOpened ? CursorLockMode.None : CursorLockMode.Locked;
        //Cursor.visible = isOpened;

        quickSlots.SetActive(IsChestOpened ? !IsOpened : true);
        //inventory.transform.localPosition = new Vector3(0, IsChestOpened ? -250 : 0, 0);
        //if (chestInventory.activeSelf)
        //{
        //    OpenedChest.SetSlots(chestInventorySlots.ToArray());
        //}
        chestInventory.SetActive(IsChestOpened ? IsOpened : false);

        IsChestOpened = false;
    }

    public void OpenChest()
    {
        IsChestOpened = true;
        ToggleInventory();
    }

    public void CloseChest()
    {
        IsChestOpened = false;
        if (IsOpened)
        {
            ToggleInventory();
        }
    }
}
