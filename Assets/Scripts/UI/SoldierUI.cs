using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldierUI : MonoBehaviour
{
    public Soldier soldier;
    public int amountRemaining = 0;
    public float price;
    public AudioClip soundEffect;
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

        priceTXT.text = price.ToString();
        cpmTXT.text = soldier.cpm.ToString();
        amountRemainingTXT.text = amountRemaining.ToString();
    }

    public void Purchase()
    {
        //can afford
        if (GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury >= price && amountRemaining > 0)
        {
            SettlementInspector.instance.GetComponent<AudioSource>().clip = soundEffect;
            SettlementInspector.instance.GetComponent<AudioSource>().Play();
            Inventory.instance.army.GetComponent<Army>().soldiers.Add(Instantiate(soldier));
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
