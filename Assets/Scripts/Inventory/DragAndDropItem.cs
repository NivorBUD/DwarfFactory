using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;
/// IPointerDownHandler - Следит за нажатиями мышки по объекту на котором висит этот скрипт
/// IPointerUpHandler - Следит за отпусканием мышки по объекту на котором висит этот скрипт
/// IDragHandler - Следит за тем не водим ли мы нажатую мышку по объекту <summary>
/// IPointerDownHandler - Следит за нажатиями мышки по объекту на котором висит этот скрипт

public class DragAndDropItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public InventorySlot oldSlot;
    private Transform player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        oldSlot = transform.GetComponentInParent<InventorySlot>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;
        //GetComponent<RectTransform>().position += new Vector3(eventData.delta.x / 100, eventData.delta.y / 100);
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GetComponent<RectTransform>().position = new Vector3(pos.x, pos.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;
        GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.75f);
        GetComponentInChildren<Image>().raycastTarget = false;
        transform.SetParent(transform.parent.parent.parent);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;
        GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
        GetComponentInChildren<Image>().raycastTarget = true;

        transform.SetParent(oldSlot.transform);
        transform.position = oldSlot.transform.position;
        if (eventData.pointerCurrentRaycast.gameObject.name == "UIBG")
        {
            //GameObject itemObject = Instantiate(oldSlot.Item.itemPrefab, player.position + Vector3.up + player.forward, Quaternion.identity);
            //itemObject.GetComponent<Item>().amount = oldSlot.Amount;
            //oldSlot.ResetData();
            return;
        }
        else if (eventData.pointerCurrentRaycast.gameObject.name == "Inventory")
        {
            return;
        }
        else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>() != null)
        {
            ExchangeSlotData(eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>());
        }
        else if (Input.GetKey(KeyCode.LeftControl) && InventoryManager.Instance.IsChestOpened)
        {
            if (oldSlot.gameObject.GetComponent<ChestSlot>() != null)
            {
                int remained = InventoryManager.Instance.TryAddItem(oldSlot.Item, oldSlot.Amount);
                if (remained == 0)
                {
                    oldSlot.ResetData();
                }
                else
                {
                    oldSlot.PlaceItem(oldSlot.Item, remained);
                }
            }
            else
            {
                int remained = InventoryManager.Instance.TryAddItemToChest(oldSlot.Item, oldSlot.Amount);
                if (remained == 0)
                {
                    oldSlot.ResetData();
                }
                else
                {
                    oldSlot.PlaceItem(oldSlot.Item, remained);
                }
            }
            InventoryManager.Instance.OpenedChest.
                SetSlots(InventoryManager.Instance.GetChestInventorySlots().ToArray());
        }
    }

    void ExchangeSlotData(InventorySlot newSlot) //newSlot - куда перетаскиваем, oldSlot - откуда 
    {
        if (oldSlot == newSlot)
        {
            return;
        }
        bool isHalf = Input.GetKey(KeyCode.LeftShift);
        bool isOne = Input.GetKey(KeyCode.LeftControl);

        int deltaAmount = oldSlot.Amount;
        if (isOne)
        {
            deltaAmount = 1;
        }
        else if (isHalf)
        {
            deltaAmount = oldSlot.Amount / 2 + 1;
        }

        if (oldSlot.Item == newSlot.Item || newSlot.Item == null)
        {
            ExchangeOneTypeItem(newSlot, isHalf, isOne, deltaAmount);
        }
        else
        {
            ExchangeDifferentTypeItem(newSlot, newSlot.Item, newSlot.Amount, newSlot.isEmpty);
        }

        if (InventoryManager.Instance.IsChestOpened)
        {
            InventoryManager.Instance.OpenedChest.
                SetSlots(InventoryManager.Instance.GetChestInventorySlots().ToArray());
        }
    }

    private void ExchangeDifferentTypeItem(InventorySlot newSlot, ItemScriptableObject item, int amount, bool isEmpty)
    {
        newSlot.PlaceItem(oldSlot.Item, oldSlot.Amount);

        newSlot.isEmpty = oldSlot.isEmpty;

        oldSlot.PlaceItem(item, amount);
        if (isEmpty)
        {
            oldSlot.iconImage.color = new Color(1, 1, 1, 0);
            oldSlot.iconImage.sprite = null;
            oldSlot.textAmount.text = "";
        }
        oldSlot.isEmpty = isEmpty;
    }

    private void ExchangeOneTypeItem(InventorySlot newSlot, bool isHalf, bool isOne, int amount)
    {
        if (newSlot.isEmpty)
        {
            newSlot.PlaceItem(oldSlot.Item, amount);
            oldSlot.PlaceItem(oldSlot.Item, oldSlot.Amount - amount);
            return;
        }
        
        if (newSlot.EmptyAmount > amount)
        {
            newSlot.AddAmount(amount);
            if (!(isOne || isHalf))
            {
                oldSlot.ResetData();
            }
            else
            {
                oldSlot.PlaceItem(oldSlot.Item, oldSlot.Amount - amount);
            }
        }
        else
        {
            oldSlot.PlaceItem(oldSlot.Item, oldSlot.Amount - newSlot.EmptyAmount);
            newSlot.AddAmount(newSlot.EmptyAmount);
        }
    }
}
