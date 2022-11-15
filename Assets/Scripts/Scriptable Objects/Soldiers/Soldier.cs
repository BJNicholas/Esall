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
    public bool supportUnit;
    public float damage;
    public float health;
    public float maxHealth;
    [Range(0,100)]public float charge = 0f; //0-100 always starts on 0
    public float chargeSpeed;


    private void Awake()
    {
        health = maxHealth;
    }

}
