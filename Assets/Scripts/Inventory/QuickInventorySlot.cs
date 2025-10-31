using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickInventorySlot : InventorySlot
{
    public bool IsActive {  get; private set; }

    public void ChangeActive()
    {
        IsActive = !IsActive;
        if (IsActive)
        {
            BGImage.color = new Color(1, 0.6f, 0.6f, 1);
        }
        else
        {
            BGImage.color = new Color(1, 1, 1, 1);
        }

        if (BuildingsGrid.Instance.IsPlacingBuilding)
        {
            BuildingsGrid.Instance.StopPlacingBuilding();
        }
    }
}
