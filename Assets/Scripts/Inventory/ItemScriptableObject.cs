using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Default,
    Weapon
}

public class ItemScriptableObject : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public int maximumAmount;
    public ItemType itemType;
    public GameObject itemPrefab;
    public Sprite icon;
}
