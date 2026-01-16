using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

// Класс для хранения данных одного слота без UI
[System.Serializable]
public class ChestSlotData
{
    public ItemScriptableObject item;
    public int amount;

    public ChestSlotData()
    {
        item = null;
        amount = 0;
    }

    public ChestSlotData(ItemScriptableObject item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}

public class Chest : Building
{
    // Хранилище данных сундука (отдельное для каждого сундука)
    private List<ChestSlotData> chestData = new List<ChestSlotData>();
    private AudioSource audioSource;
    private AudioClip openChestSound;
    private AudioClip closeChestSound;
    private int slotCount = 0;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        openChestSound = Resources.Load<AudioClip>("Sounds/OpenChest");
        closeChestSound = Resources.Load<AudioClip>("Sounds/CloseChest");
    }



    public void InizializeUISlotsFromSlotsList(List<InventorySlot> chestSlots)
    {
        // Первая инициализация - создаем пустое хранилище
        if (chestData.Count == 0)
        {
            slotCount = chestSlots.Count;
            for (int i = 0; i < slotCount; i++)
            {
                chestData.Add(new ChestSlotData());
            }
        }

        // Загружаем данные этого сундука в UI слоты
        for (int i = 0; i < chestSlots.Count && i < chestData.Count; i++)
        {
            chestSlots[i].Set(chestData[i].item, chestData[i].amount);
        }
    }

    public int AddItems(ItemScriptableObject item, int amount)
    {
        // Добавляем в существующие стаки
        for (int i = 0; i < chestData.Count; i++)
        {
            if (amount <= 0) return 0;

            if (chestData[i].item == item && chestData[i].amount < item.maximumAmount)
            {
                int space = item.maximumAmount - chestData[i].amount;
                int addAmount = Mathf.Min(amount, space);
                chestData[i].amount += addAmount;
                amount -= addAmount;
            }
        }

        // Добавляем в пустые слоты
        for (int i = 0; i < chestData.Count; i++)
        {
            if (amount <= 0) return 0;

            if (chestData[i].item == null)
            {
                int placeAmount = Mathf.Min(amount, item.maximumAmount);
                chestData[i].item = item;
                chestData[i].amount = placeAmount;
                amount -= placeAmount;
            }
        }

        return amount;
    }

    public void SaveData(List<InventorySlot> newSlots)
    {
        // Сохраняем данные из UI слотов в хранилище этого сундука
        for (int i = 0; i < newSlots.Count && i < chestData.Count; i++)
        {
            chestData[i].item = newSlots[i].Item;
            chestData[i].amount = newSlots[i].Amount;
        }
    }

    public override void interaction()
    {
        InventoryManager.Instance.OpenChest(this);
    }

    public void PlayOpenSound()
    {
        if (audioSource != null && openChestSound != null)
        {
            audioSource.PlayOneShot(openChestSound);
        }
    }

    public void PlayCloseSound()
    {
        if (audioSource != null && closeChestSound != null)
        {
            audioSource.PlayOneShot(closeChestSound);
        }
    }
}
