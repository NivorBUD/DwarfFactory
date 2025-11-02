using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Chest : Building
{
    public ChestSlot[] Slots { get; private set; }
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

    public void InizializeUISlotsFromParentObj(GameObject parent)
    {
        List<InventorySlot> slots = new ();
        bool isSetSlot = inventoryContainer.Slots.Count == parent.transform.childCount;
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if (parent.transform.GetChild(i).TryGetComponent(out InventorySlot slot))
            {
                if (isSetSlot)
                {
                    slot.Set(inventoryContainer.Slots[i].Item, inventoryContainer.Slots[i].Amount);
                }
                else
                {
                    slots.Add(slot);
                }
            }
        }

        inventoryContainer.SetNewSlots(slots);
    }

    public int AddItems(ItemScriptableObject item, int amount)
        => inventoryContainer.AddItems(item, amount);

    public void SetSlots(InventorySlot[] newSlots)
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].PlaceItem(newSlots[i].Item, newSlots[i].Amount);
        }
    }

    public override void interaction()
    {
        InventoryManager.Instance.OpenChest(this);
    }
}
