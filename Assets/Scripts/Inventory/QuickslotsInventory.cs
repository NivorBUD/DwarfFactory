using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class QuickslotsInventory : MonoBehaviour
{
    public static QuickslotsInventory Instance;

    private List<QuickInventorySlot> slots;
    private int currentQuickslotID = 0;
    private QuickInventorySlot activeSlot;

    private void Awake()
    {
        Instance = this;
        slots = new();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<QuickInventorySlot>() != null)
            {
                slots.Add(transform.GetChild(i).GetComponent<QuickInventorySlot>());
            }
        }
    }

    private void Update()
    {
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        if (mouseWheel > 0.1)
            ScrollUp();

        if (mouseWheel < -0.1)
            ScrollDown();

        CheckNums();

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (BuildingsGrid.Instance.IsPlacingBuilding)
            {
                BuildingsGrid.Instance.StopPlacingBuilding();
            }
            else if (activeSlot != null && activeSlot.Slot.Item != null && activeSlot.Slot.Item.itemType == ItemType.Building)
            {
                BuildingsGrid.Instance.StartPlacingBuilding(activeSlot.Slot.Item.itemPrefab.GetComponent<Building>());
            }
        }

        if (BuildingsGrid.Instance.IsPlacingBuilding && activeSlot.Slot.Amount == 0)
        {
            BuildingsGrid.Instance.StopPlacingBuilding();
        }
    }

    public void RemoveUsedItemFromActiveSlot()
    {
        activeSlot.Slot.RemoveAmount(1);
    }

    private void CheckNums()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                if (slots[currentQuickslotID].IsActive && currentQuickslotID != i)
                {
                    slots[currentQuickslotID].ChangeActive();
                }
                currentQuickslotID = i;
                slots[currentQuickslotID].ChangeActive();
                activeSlot = slots[currentQuickslotID];
            }
        }
    }

    private void ScrollDown()
    {
        if (slots[currentQuickslotID].IsActive)
        {
            slots[currentQuickslotID].ChangeActive();
        }

        currentQuickslotID = currentQuickslotID <= 0 ?
            slots.Count - 1 : currentQuickslotID - 1;

        slots[currentQuickslotID].ChangeActive();
        activeSlot = slots[currentQuickslotID];
    }

    private void ScrollUp()
    {
        if (slots[currentQuickslotID].IsActive)
        {
            slots[currentQuickslotID].ChangeActive();
        }

        currentQuickslotID = currentQuickslotID >= slots.Count - 1 ? 
            0 : currentQuickslotID + 1;

        slots[currentQuickslotID].ChangeActive();
        activeSlot = slots[currentQuickslotID];
    }
}
