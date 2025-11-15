using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Chest : Building
{
    private InventoryContainer inventoryContainer;

    private void Start()
    {
        inventoryContainer = new();
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                InventoryManager.Instance.OpenChest(this);
            }
        }
    }

    public void InizializeUISlotsFromSlotsList(List<InventorySlot> chestSlots)
    {
        bool isSlotsSet = inventoryContainer.Slots.Count == chestSlots.Count;
        List<InventorySlot> slots = new();
        for (int i = 0; i < chestSlots.Count; i++)
        {
            if (isSlotsSet)
            {
                chestSlots[i].Set(inventoryContainer.Slots[i].Item, inventoryContainer.Slots[i].Amount);
            }
            else
            {
                chestSlots[i].Clear();
                slots.Add(chestSlots[i].Copy());
            }
        }
        if (!isSlotsSet)
        {
            inventoryContainer.SetNewSlots(slots);
        }
    }

    public int AddItems(ItemScriptableObject item, int amount)
        => inventoryContainer.AddItems(item, amount);

    public void SaveData(List<InventorySlot> newSlots)
    {
        for (int i = 0; i < inventoryContainer.Slots.Count; i++)
        {
            inventoryContainer.Slots[i].Set(newSlots[i].Item, newSlots[i].Amount);
        }
    }

    public override void interaction()
    {
        InventoryManager.Instance.OpenChest(this);
    }
}
