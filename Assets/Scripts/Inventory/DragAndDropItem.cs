using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;
/// IPointerDownHandler - ������ �� ��������� ����� �� ������� �� ������� ����� ���� ������
/// IPointerUpHandler - ������ �� ����������� ����� �� ������� �� ������� ����� ���� ������
/// IDragHandler - ������ �� ��� �� ����� �� �� ������� ����� �� ������� <summary>
/// IPointerDownHandler - ������ �� ��������� ����� �� ������� �� ������� ����� ���� ������

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
        if (oldSlot.IsEmpty)
            return;
        //GetComponent<RectTransform>().position += new Vector3(eventData.delta.x / 100, eventData.delta.y / 100);
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GetComponent<RectTransform>().position = new Vector3(pos.x, pos.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (oldSlot.IsEmpty)
            return;
        GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.75f);
        GetComponentInChildren<Image>().raycastTarget = false;
        transform.SetParent(transform.parent.parent.parent);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (oldSlot.IsEmpty)
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
    }

    void ExchangeSlotData(InventorySlot newSlot) //newSlot - to, oldSlot - from 
    {
        if (newSlot.gameObject.TryGetComponent<SpecificItemSlot>(out var specSlot))
        {
            if (specSlot.AllowedItem != oldSlot.Item)
                return;
        }

        if (oldSlot == newSlot)
        {
            return;
        }
        bool isHalf = InputHandler.Instance != null && InputHandler.Instance.IsShiftHeld;
        bool isOne = InputHandler.Instance != null && InputHandler.Instance.IsControlHeld;

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
            ExchangeDifferentTypeItem(newSlot, newSlot.Item, newSlot.Amount, newSlot.IsEmpty);
        }

        if (InventoryManager.Instance.IsChestOpened)
        {
            InventoryManager.Instance.SaveChestInventory();
        }
    }

    private void ExchangeDifferentTypeItem(InventorySlot newSlot, ItemScriptableObject item, int amount, bool isEmpty)
    {
        newSlot.Set(oldSlot.Item, oldSlot.Amount);

        oldSlot.Set(item, amount);
        
        if (isEmpty)
        {
            oldSlot.Clear();
        }
    }

    private void ExchangeOneTypeItem(InventorySlot newSlot, bool isHalf, bool isOne, int amount)
    {
        if (newSlot.IsEmpty)
        {
            newSlot.Set(oldSlot.Item, amount);

            oldSlot.Set(oldSlot.Item, oldSlot.Amount - amount);

            return;
        }
        
        if (newSlot.EmptyAmount > amount)
        {
            newSlot.AddAmount(amount);

            if (!(isOne || isHalf))
            {
                oldSlot.Clear();
            }
            else
            {
                oldSlot.Set(oldSlot.Item, oldSlot.Amount - amount);
            }
        }
        else
        {
            oldSlot.Set(oldSlot.Item, oldSlot.Amount - newSlot.EmptyAmount);
            newSlot.AddAmount(newSlot.EmptyAmount);
        }
    }
}
