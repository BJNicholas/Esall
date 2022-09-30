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
    public float sellValue;
    public float purchasePrice;
    public List<Item> storedItems;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        UpdateOwner(owner);
        currentSate = possibleStates. Idle;
        purchasePrice = 0;
        BuyAndSell(homeCity, true);
        Idle();

        transform.rotation = Quaternion.identity; // fixes rotation bug that makes armies/merhcants invisible
    }

    private void Update()
    {
        agent.SetDestination(target.position);
        stateTXT.text = currentSate.ToString();

        if(Vector3.Distance(target.position,transform.position) <= 0.5f)
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
            BuyAndSell(settlement, false);
            Invoke("ReturnHome", 2f);
        }
        else if (currentSate == possibleStates.Returning)
        {
            currentSate = possibleStates.Trading;
            BuyAndSell(settlement, true);
            Invoke("Idle", 2f);
        }

    }
    void ReturnHome()
    {
        target = homeCity.transform;
        currentSate = possibleStates.Returning;
    }

    void BuyAndSell(GameObject settlement, bool home)
    {
        if (!home)
        {
            //selling
            sellValue = 0;
            float saleProfit = 0;
            foreach (Item item in storedItems)//items stored in the merchant's inventory
            {
                float numAlreadyStored = 0;
                foreach (Item storedItem in settlement.GetComponent<Settlement>().storedItems)
                {
                    if (storedItem == item) numAlreadyStored += 1;
                }
                saleProfit += item.baseValue;
                settlement.GetComponent<Settlement>().storedItems.Add(item);
                //print("sold " + item.name + " for a profit of: " + (localPrice - item.baseValue).ToString());
            }
            sellValue += (saleProfit + saleProfit / 2);
            print(sellValue + " and bought for " + purchasePrice);
            storedItems.Clear();

            PayTax();
        }
        else
        {
            sellValue = 0;
        }

        if (home)
        {
            //buying
            purchasePrice = 0;
            List<Item> desiredItems = new List<Item>();
            desiredItems.Add(settlement.GetComponent<Settlement>().storedItems[0]);
            foreach (Item item in settlement.GetComponent<Settlement>().storedItems)//items stored in the settlements market
            {
                if (settlement.GetComponent<Settlement>().producedItems.Contains(item))
                {
                    if (!desiredItems.Contains(item))
                    {
                        desiredItems.Add(item);
                    }
                }
            }
            //print("DESIRES: " + desiredItems.ToArray().Length + " Items");

            if (desiredItems.ToArray().Length > 0)
            {
                foreach (Item item in desiredItems) // purchasing items
                {
                    storedItems.Add(item);
                    settlement.GetComponent<Settlement>().storedItems.Remove(item);

                    purchasePrice += item.baseValue;
                }
            }
            //print("after purchase, I have " + treasury.ToString() + " due to a cost of " + (oldTreasuryValue - treasury).ToString());
            ownerObject.GetComponent<Faction>().treasury += (purchasePrice);
            //print(owner.ToString() + " has collected: $" + (purchasePrice).ToString() + " from the purchase of Items in " + homeCity.ToString());
        }
        else
        {
            purchasePrice = 0;
        }

        //update the trading hub if it is open by player
        if (TradingHub.instance.gameObject.activeInHierarchy && TradingHub.instance.settlement == settlement)
        {
            TradingHub.instance.LiveUpdateListings();
        }
    }

    void PayTax()
    {
        float profit = sellValue - purchasePrice;
        if(profit > 0)
        {
            ownerObject.GetComponent<Faction>().treasury += (profit * 0.5f); // maybe have an ajustable tax rate later
        }
        //print(owner.ToString() + " has collected: $" + (profit * 0.5f).ToString() + " in taxes from a merchant of " + homeCity.ToString());
    }





}
