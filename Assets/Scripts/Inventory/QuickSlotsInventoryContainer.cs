using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class QuickSlotsInventoryContainer : InventoryContainer
{
    private int currentQuickSlotID = 0;
    public InventorySlot activeSlot { get; private set; }

    public QuickSlotsInventoryContainer(GameObject parent) : base(parent)
    {
    }

    public void ChangeActiveSlotTo(int index)
    {
        currentQuickSlotID = index;

        for (int i = 0; i < slots.Count; ++i)
        {
            if (index == i)
            {
                activeSlot = slots[i];
                activeSlot.ChangBG(new Color(1, 0.6f, 0.6f, 1));
                continue;
            }
            slots[i].ChangeBGToStandard();
        }

        if (BuildingsGrid.Instance.IsPlacingBuilding)
        {
            BuildingsGrid.Instance.StopPlacingBuilding();
        }
    }

    public void RemoveUsedItemFromActiveSlot()
    {
        activeSlot.RemoveAmount(1);
    }

    public void CheckNums()
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                ChangeActiveSlotTo(i);
                break;
            }
        }
    }

    public void ScrollDown()
    {
        int activeSlotId = currentQuickSlotID <= 0 ?
            Slots.Count - 1 : currentQuickSlotID - 1;

        ChangeActiveSlotTo(activeSlotId);
    }

    public void ScrollUp()
    {
        int activeSlotId = currentQuickSlotID >= Slots.Count - 1 ?
            0 : currentQuickSlotID + 1;

        ChangeActiveSlotTo(activeSlotId);
    }
}
