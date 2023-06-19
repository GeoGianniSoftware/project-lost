using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [System.NonSerialized]
    public Collider COL;
    public ItemStats itemRef;
    public bool beingHeld = false;
    public Entity holder;




}

[System.Serializable]
public class ItemStats
{
    public string itemName;
    public ItemType itemType;

    public ItemStats(string name, ItemType type) {
        itemName = name;
        itemType = type;
    }
}


public enum ItemType
{
    Resource,
    Weapon
}
