using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public enum ItemType
    {
        resource,
        food,
    }
    public int id;
    public new string name;
    public ItemType type;
    public string description;

    public Sprite icon;

    public float baseValue;
}
