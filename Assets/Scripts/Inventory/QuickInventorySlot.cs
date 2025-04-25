using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickInventorySlot : MonoBehaviour
{
    [SerializeField] private Image nonActiveSprite, activeSprite;

    public bool IsActive {  get; private set; }
    public InventorySlot Slot { get; private set; }

    private void Awake()
    {
        Slot = GetComponent<InventorySlot>();
    }

    public void ChangeActive()
    {
        IsActive = !IsActive;
        if (IsActive)
        {
            Slot.BGImage.color = new Color(1, 0.6f, 0.6f, 1);
        }
        else
        {
            Slot.BGImage.color = new Color(1, 1, 1, 1);
        }

        if (BuildingsGrid.Instance.IsPlacingBuilding)
        {
            BuildingsGrid.Instance.StopPlacingBuilding();
        }
    }
}
