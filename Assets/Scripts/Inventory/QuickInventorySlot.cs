using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickInventorySlot : MonoBehaviour
{
    [SerializeField] private Image nonActiveSprite, activeSprite;

    public bool IsActive {  get; private set; }
    private InventorySlot slot;

    private void Awake()
    {
        slot = GetComponent<InventorySlot>();
    }

    public void ChangeActive()
    {
        IsActive = !IsActive;
        if (IsActive)
        {
            slot.BGImage.color = new Color(1, 0.6f, 0.6f, 1);
        }
        else
        {
            slot.BGImage.color = new Color(1, 1, 1, 1);
        }
    }
}
