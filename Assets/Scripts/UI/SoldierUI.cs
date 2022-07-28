using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldierUI : MonoBehaviour
{
    public Soldier soldier;
    public int amountRemaining = 0;
    public float price;
    [Header("UI Setup")]
    public Image icon;
    public Text itemName, priceTXT, cpmTXT, amountRemainingTXT;


    private void Start()
    {
        icon.sprite = soldier.icon;
        itemName.text = soldier.name;

        gameObject.name = soldier.ToString();
    }

    private void Update()
    {

        priceTXT.text = price.ToString("0.00");
        cpmTXT.text = soldier.cpm.ToString("0.00");
        amountRemainingTXT.text = amountRemaining.ToString();
    }

    public void Purchase()
    {
        //can afford
        if (GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury >= price && amountRemaining > 0)
        {
            Inventory.instance.army.GetComponent<Army>().soldiers.Add(soldier);
            Inventory.instance.LiveUpdateListings();

            RecruitmentHub.instance.settlement.GetComponent<Settlement>().availableSoldiers.Remove(soldier);
            RecruitmentHub.instance.LiveUpdateListings();

            GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury -= price;
            amountRemaining -= 1;

            GameManager.instance.playerFactionObject.GetComponent<Faction>().expenses += soldier.cpm;
        }
        else
        {
            print("Can't Afford: " + soldier.name);
        }
    }
}
