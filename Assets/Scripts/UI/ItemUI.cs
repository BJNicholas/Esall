using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Item item;
    public int amountRemaining = 0;
    public float price;
    [Header("UI Setup")]
    public Image icon;
    public Text itemName, priceTXT, amountRemainingTXT;


    private void Start()
    {
        icon.sprite = item.icon;
        itemName.text = item.name;

        gameObject.name = item.ToString();
        
    }

    private void Update()
    {
        priceTXT.text = price.ToString("0.00");
        amountRemainingTXT.text = amountRemaining.ToString();
    }

    public void Purchase()
    {
        //can afford
        if(GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury >= price && amountRemaining > 0)
        {
            Inventory.instance.army.GetComponent<Army>().storedItems.Add(item);
            Inventory.instance.LiveUpdateListings();

            TradingHub.instance.settlement.GetComponent<Settlement>().storedItems.Remove(item);
            TradingHub.instance.LiveUpdateListings();

            GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury -= price;
            amountRemaining -= 1;
        }
        else
        {
            print("Can't Afford: " + item.name);
        }
    }
    public void Sell()
    {
        if (TradingHub.instance.gameObject.activeInHierarchy)
        {
            TradingHub.instance.settlement.GetComponent<Settlement>().storedItems.Add(item);
            TradingHub.instance.LiveUpdateListings();

            Inventory.instance.army.GetComponent<Army>().storedItems.Remove(item);
            Inventory.instance.LiveUpdateListings();

            GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury += price;
            amountRemaining -= 1;
        }
        else print("NEED TO TRAVEL TO A TRADING POST");
    }
}