using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSlot : MonoBehaviour
{
    public ItemScriptableObject Item { get; private set; }
    public int Amount { get; private set; }

    public bool isEmpty = true;

    public void PlaceItem(ItemScriptableObject item, int amount)
    {
        isEmpty = false;
        Item = item;
        Amount = amount;
        if (amount == 0)
            isEmpty = true;
    }

    public void AddAmount(int amount)
    {
        Amount += amount;
    }

    public void UpdateInfo(ItemScriptableObject item, int amount)
    {
        Item = item;
        Amount = amount;
    }
}
