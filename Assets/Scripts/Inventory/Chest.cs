using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public ChestSlot[] Slots { get; private set; }

    private InventoryManager inventoryManager;
    private bool isOpen, isPlayerNear;

    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
        Slots = new ChestSlot[21];
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i] = new();
            Slots[i].isEmpty = true;
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                inventoryManager.OpenChest(this);
            }
        }
    }

    public void AddItem(ItemScriptableObject item, int amount)
    {
        foreach (ChestSlot slot in Slots)
        {
            if (slot.isEmpty)
            {
                slot.PlaceItem(item, amount);
                if (isOpen && isPlayerNear)
                {
                    inventoryManager.GoToTheChest(this);
                }
                return;
            }

            if (slot.Item == item && slot.Amount < item.maximumAmount)
            {
                if (slot.Amount + amount <= item.maximumAmount)
                    slot.AddAmount(amount);
                else
                {
                    amount -= item.maximumAmount - slot.Amount;
                    slot.AddAmount(item.maximumAmount - slot.Amount);
                    AddItem(item, amount);
                }

                if (isOpen && isPlayerNear)
                {
                    inventoryManager.GoToTheChest(this);
                }
                return;
            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        inventoryManager.GoToTheChest(this);
    //        isPlayerNear = true;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        inventoryManager.GoAwayFromTheChest();
    //        isPlayerNear = false;
    //    }
    //}

    public void SetSlots(InventorySlot[] newSlots)
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].PlaceItem(newSlots[i].Item, newSlots[i].Amount);
        }
    }

    public void SetIsOpen(bool newValue)
    {
        isOpen = newValue;
    }
}
