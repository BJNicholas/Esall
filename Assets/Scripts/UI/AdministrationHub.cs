using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdministrationHub : MonoBehaviour
{
    public static AdministrationHub instance;
    [Header("stats")]
    public GameObject settlement;

    [Header("UI Setup")]
    public Text settlementName;
    public Text development, garrison, merchants;
    public Text devPRICE, merchantPRICE;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        settlement = SettlementInspector.instance.settlement;
        settlementName.text = settlement.GetComponent<Settlement>().settlementName;
        development.text = settlement.GetComponent<Settlement>().province.GetComponent<Tile>().development.ToString();
        merchants.text = settlement.GetComponent<Settlement>().merchants.ToArray().Length.ToString();

        //prices
        devPRICE.text = FindDevPrice(0f, settlement).ToString();
        merchantPRICE.text = FindMerchantPrice(0f, settlement).ToString();
    }

    public float FindDevPrice(float value, GameObject settlement)
    {
        value = 10; // base value
        value *= settlement.GetComponent<Settlement>().province.GetComponent<Tile>().development;


        return value;
    }
    public float FindMerchantPrice(float value, GameObject settlement)
    {
        value = 100; // base value
        if(settlement.GetComponent<Settlement>().merchants.ToArray().Length > 0)
        {
            value *= settlement.GetComponent<Settlement>().merchants.ToArray().Length;
        }
        value -= settlement.GetComponent<Settlement>().province.GetComponent<Tile>().development * 10;


        return value;
    }

    public void RaiseDEV()
    {
        //can afford
        if (GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury >= FindDevPrice(0f, settlement))
        {
            GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury -= FindDevPrice(0f, settlement);
            settlement.GetComponent<Settlement>().province.GetComponent<Tile>().development += 1;
            settlement.GetComponent<Settlement>().province.GetComponent<Tile>().taxIncome += settlement.GetComponent<Settlement>().province.GetComponent<Tile>().development / 10;
            print("Tax income in " + settlementName.text + " increased by: " + settlement.GetComponent<Settlement>().province.GetComponent<Tile>().development / 10);
            GameManager.instance.playerFactionObject.GetComponent<Faction>().UpdateOwnedTiles();
            GameManager.instance.playerFactionObject.GetComponent<Faction>().CalculateTax();

        }
        else
        {
            print("CAN NOT AFFORD TO RAISE DEVELOPMENT");
        }
    }
    public void PurchaseMER()
    {
        //can afford
        if (GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury > FindMerchantPrice(0f, settlement))
        {
            GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury -= FindMerchantPrice(0f, settlement);
            settlement.GetComponent<Settlement>().NewMerchant();
        }
        else
        {
            print("CAN NOT AFFORD TO PURCHASE MERCHANT");
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
