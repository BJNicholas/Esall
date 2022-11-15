using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Soldier", menuName = "Soldier")]
public class Soldier : ScriptableObject
{
    public int id;
    public new string name;
    public string description;
    public GameObject Prefab;

    public Sprite icon;
    [Header("Cost")]
    [Tooltip("Price on initial purchase")] public float buyPrice; // price on initial purchase
    [Tooltip("Cost per Month")] public float cpm; // cost per month

    [Header("Stats")]
    public float heath;
    public float maxHealth;


    private void Awake()
    {
        heath = maxHealth;
    }

}
