using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Merchant : MonoBehaviour
{
    public enum possibleStates
    {
        Idle,
        Travelling,
        Returning,
        Trading
    }

    public Text stateTXT;
    [Header("Details")]
    public GameObject homeCity;
    public FactionManager.factions owner;
    [HideInInspector] public GameObject ownerObject;
    public possibleStates currentSate;
    [Header("NAV MESH")]
    public Transform target;
    private NavMeshAgent agent;
    [Header("Trading")]
    public float treasury = 10;
    public float oldTreasuryValue;
    public List<Item> storedItems;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        UpdateOwner(owner);
        currentSate = possibleStates. Idle;
        oldTreasuryValue = treasury;
        BuyAndSell(homeCity);
        Idle();

        transform.rotation = Quaternion.identity; // fixes rotation bug that makes armies/merhcants invisible
    }

    private void Update()
    {
        agent.SetDestination(target.position);
        stateTXT.text = currentSate.ToString();

        if(Vector3.Distance(target.position,transform.position) <= 0.2f)
        {
            Trade(target.gameObject);
        }
    }

    public void UpdateOwner(FactionManager.factions newOwner)
    {
        owner = newOwner;
        foreach(GameObject faction in FactionManager.instance.factionObjects)
        {
            if(faction.GetComponent<Faction>().faction == owner)
            {
                Color32 colour = faction.GetComponent<Faction>().colour;
                GetComponent<SpriteRenderer>().color = new Color32(colour.r, colour.g, colour.b, 200);
                ownerObject = faction;
            }
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(collision.gameObject.transform == target)
    //    {
    //        Trade(collision.gameObject);
    //    }
    //}

    void Idle()
    {
        int roll = Random.Range(0, GameManager.instance.settlements.ToArray().Length);
        if(GameManager.instance.settlements[roll] == homeCity)
        {
            Idle();
        }
        else
        {
            target = GameManager.instance.settlements[roll].transform;
            currentSate = possibleStates.Travelling;
        }
    }
    void Trade(GameObject settlement)
    {
        if (currentSate == possibleStates.Travelling)
        {
            currentSate = possibleStates.Trading;

            //hand over goods
            BuyAndSell(settlement);
            Invoke("ReturnHome", 2f);
        }
        else if (currentSate == possibleStates.Returning)
        {
            currentSate = possibleStates.Trading;
            BuyAndSell(settlement);
            Invoke("Idle", 2f);
        }

    }
    void ReturnHome()
    {
        target = homeCity.transform;
        currentSate = possibleStates.Returning;
    }

    void BuyAndSell(GameObject settlement)
    {
        //selling
        float saleProfit = 0;
        foreach(Item item in storedItems)//items stored in the merchant's inventory
        {
            float numAlreadyStored = 0;
            foreach(Item storedItem in settlement.GetComponent<Settlement>().storedItems)
            {
                if (storedItem == item) numAlreadyStored += 1;
            }
            float localPrice = item.baseValue;
            localPrice += 0.1f * (10 - numAlreadyStored);
            if (localPrice <= item.baseValue) localPrice = item.baseValue;
            saleProfit += item.baseValue + (localPrice - item.baseValue);
            settlement.GetComponent<Settlement>().storedItems.Add(item);
            //print("sold " + item.name + " for a profit of: " + (localPrice - item.baseValue).ToString());
        }
        treasury += saleProfit;
        //print("after sale, I have " + treasury.ToString() + " due to a profit of " + (saleProfit).ToString());
        PayTax();
        storedItems.Clear();
        //buying
        List<Item> desiredItems = new List<Item>();
        desiredItems.Add(settlement.GetComponent<Settlement>().storedItems[0]);
        foreach(Item item in settlement.GetComponent<Settlement>().storedItems)//items stored in the settlements market
        {
            if (desiredItems.Contains(item));
            else desiredItems.Add(item);
        }
        //print("DESIRES: " + desiredItems.ToArray().Length + " Items");

        if(desiredItems.ToArray().Length > 0)
        {
            foreach (Item item in desiredItems) // purchasing items
            {
                if (treasury >= item.baseValue)
                {
                    storedItems.Add(item);
                    settlement.GetComponent<Settlement>().storedItems.Remove(item);

                    treasury -= item.baseValue;
                }
            }
        }
        //print("after purchase, I have " + treasury.ToString() + " due to a cost of " + (oldTreasuryValue - treasury).ToString());


        //update the trading hub if it is open by player
        if(TradingHub.instance.gameObject.activeInHierarchy && TradingHub.instance.settlement == settlement)
        {
            TradingHub.instance.LiveUpdateListings();
        }
    }

    void PayTax()
    {
        float profit = treasury - oldTreasuryValue;
        if(profit > 0)
        {
            ownerObject.GetComponent<Faction>().treasury += (profit * 0.5f); // maybe have an ajustable tax rate later
            treasury -= (profit * 0.5f);
        }

        oldTreasuryValue = treasury;
    }





}
