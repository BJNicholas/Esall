using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldierInventoryUI : MonoBehaviour
{
    public Soldier soldier;


    [Header("UI Setup")]
    public Image icon;
    public Text soldierNameText, healthText;
    public Slider healthBar;


    private void Start()
    {
        icon.sprite = soldier.icon;
        soldierNameText.text = soldier.name;

        healthBar.maxValue = soldier.maxHealth;
    }



    public void Update()
    {
        healthText.text = soldier.health.ToString();
        healthBar.value = soldier.health;

        if(soldier.health <= 0)
        {
            Destroy(gameObject);
        }

    }

    public void Disband()
    {
        if(ArmyInventory.instance.army.GetComponent<Army>().numOfSoldiers > 1)
        {
            for (int i = 0; i < ArmyInventory.instance.army.GetComponent<Army>().soldiers.ToArray().Length; i++)
            {
                if (ArmyInventory.instance.army.GetComponent<Army>().soldiers[i] == soldier)
                {
                    ArmyInventory.instance.army.GetComponent<Army>().soldiers.RemoveAt(i);
                }
            }
            Destroy(gameObject);
        }
        else
        {
            print("Cant disband last unit, need at least one");
        }
        
    }
}
