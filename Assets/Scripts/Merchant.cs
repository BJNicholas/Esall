using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;

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
    public float speed = 0.1f;
    [Header("Trading")]
    public float sellValue;
    public float purchasePrice;
    public List<Item> storedItems;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        target = homeCity.transform;

        //setting the starting faction owner object
        foreach (GameObject faction in FactionManager.instance.factionObjects)
        {
            if (faction.GetComponent<Faction>().faction == owner)
            {
                Color32 colour = faction.GetComponent<Faction>().colour;
                GetComponent<SpriteRenderer>().color = new Color32(colour.r, colour.g, colour.b, 200);
                ownerObject = faction;
            }
        }
        ownerObject.GetComponent<Faction>().numOfMerchants += 1;
        UpdateOwner(owner);


        currentSate = possibleStates. Idle;
        purchasePrice = 0;
        BuyAndSell(homeCity, true);
        Idle();

        transform.rotation = Quaternion.identity; // fixes rotation bug that makes armies/merhcants invisible
    }
    [HideInInspector] public float resetTimer = 0;
    public void ResetAI()
    {
        resetTimer = 0;
        print(gameObject.name + " Is resetting");
        Idle();
    }
    private void Update()
    {
        agent.speed = speed * GameManager.instance.timeSpeed;
        agent.SetDestination(target.position);
        stateTXT.text = currentSate.ToString();

        if(Vector3.Distance(target.position,transform.position) <= 0.5f)
        {
            Trade(target.gameObject);
        }

        Vector2 heading, current;
        heading = GetComponent<NavMeshAgent>().destination;
        current = gameObject.transform.position;
        if (heading == current) resetTimer++;
        else resetTimer = 0;
        if (resetTimer >= 5000) ResetAI();

        if(ownerObject.activeInHierarchy == false)
        {
            UpdateOwner(homeCity.GetComponent<Settlement>().province.GetComponent<Tile>().owner);
        }

        if (Settings.instance.showMerchants == false)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponentInChildren<Text>().text = "";
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    public void UpdateOwner(FactionManager.factions newOwner)
    {
        ownerObject.GetComponent<Faction>().numOfMerchants -= 1;
        owner = newOwner;
        foreach (GameObject faction in FactionManager.instance.factionObjects)
        {
            if(faction.GetComponent<Faction>().faction == owner)
            {
                Color32 colour = faction.GetComponent<Faction>().colour;
                GetComponent<SpriteRenderer>().color = new Color32(colour.r, colour.g, colour.b, 200);
                ownerObject = faction;
                ownerObject.GetComponent<Faction>().numOfMerchants += 1;
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
        sellValue = 0;
        purchasePrice = 0;
        if (!home)
        {
            //selling
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
            }
            sellValue += (saleProfit + saleProfit / 2);
            storedItems.Clear();

            PayTax();
        }

        if (home)
        {
            //buying
            List<Item> desiredItems = new List<Item>();
            if (settlement.GetComponent<Settlement>().storedItems.ToArray().Length > 0)
            {
                desiredItems.Add(settlement.GetComponent<Settlement>().storedItems[0]);
                foreach (Item item in settlement.GetComponent<Settlement>().storedItems)//items stored in the settlements market
                {
                    if (settlement.GetComponent<Settlement>().producedItems.Contains(item))
                    {
                        desiredItems.Add(item);
                    }
                }
                desiredItems = desiredItems.Distinct().ToList();
            }
            else
            {
                //nothing there moving on
                //print(gameObject.name + " nothing stored in settlement, moving on");
                return;
            }
            
            if (desiredItems.ToArray().Length > 0)
            {
                foreach (Item item in desiredItems) // purchasing items
                {
                    storedItems.Add(item);
                    settlement.GetComponent<Settlement>().storedItems.Remove(item);

                    purchasePrice += item.baseValue;
                }
            }
            else
            {
                //print("No Items to buy, moving on");
                return;
            }

            ownerObject.GetComponent<Faction>().treasury += (purchasePrice);
            ownerObject.GetComponent<Faction>().monthlyTrade += purchasePrice;

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
            ownerObject.GetComponent<Faction>().monthlyTrade += (profit * 0.5f);
        }
        //print(owner.ToString() + " has collected: $" + (profit * 0.5f).ToString() + " in taxes from a merchant of " + homeCity.ToString());
    }





}
