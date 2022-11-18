using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [HideInInspector] public int id;
    public FactionManager.factions owner;
    public CultureManager.cultures culture;
    public GameObject settlement;
    public GameObject ownerObject;
    public List<GameObject> neighbouringTiles;

    [Header("Tile Stats")]
    [Range(0,10)]public float development;
    [Range(0,100)]public float publicOrder;
    public float manpowerCap;
    public float taxIncome;
    public bool coastal;

    private void Start()
    {
        TaxIncomeCalculation();
        foreach(GameObject faction in FactionManager.instance.factionObjects)
        {
            if(faction.GetComponent<Faction>().faction == owner)
            {
                ownerObject = faction;
                ownerObject.GetComponent<Faction>().ownedTiles.Add(gameObject);
            }
        }

        //calculate the starting public order
        GenerateStartingOrder();

    }

    public void TaxIncomeCalculation()
    {
        manpowerCap = 0;
        //starting tax income calculation
        for (float i = 0; i <= development; i++)
        {
            taxIncome += i * 2f;
            manpowerCap += i * 1.5f;
        }
    }

    private void Update()
    {
        //capping the values
        if (publicOrder > 100) publicOrder = 100;
        else if (publicOrder < 0) publicOrder = 0;
    }

    public void GenerateStartingOrder()
    {
        publicOrder = Random.Range(30, 80); // base value
        if (culture != ownerObject.GetComponent<Faction>().culture) publicOrder -= 10;

        publicOrder += development;

        if (settlement == ownerObject.GetComponent<Faction>().capitalCity)
        {
            publicOrder = 100;
        }
    }

    public void IncreaseDevelopment()
    {
        float price = AdministrationHub.instance.FindDevPrice(0f, settlement);
        if(ownerObject.GetComponent<Faction>().treasury >= price)
        {
            //develop
            ownerObject.GetComponent<Faction>().treasury -= price;
            development += 1;
            taxIncome += development / 10;
            ownerObject.GetComponent<Faction>().UpdateOwnedTiles();
        }
        else
        {
            //not enough money
            print(owner.ToString() + " cannot afford to develop " + settlement.GetComponent<Settlement>().settlementName);
        }

        if (development >= 5 && settlement.GetComponent<Settlement>().capital == false) settlement.GetComponent<SpriteRenderer>().sprite = settlement.GetComponent<Settlement>().city;
    }

    public void PublicOrderChange()
    {
        float change = 0;
        //cultural effect
        if (culture == ownerObject.GetComponent<Faction>().culture) change += 1.5f;
        else change -= 1.5f;
        //taxtion effect
        if (ownerObject.GetComponent<Faction>().taxRate > 1) change -= ownerObject.GetComponent<Faction>().taxRate * 2;
        else change +=  (1 - ownerObject.GetComponent<Faction>().taxRate) * 2;
        print(change + " more public order for: " + owner.ToString() + " in... " +gameObject.name);
        publicOrder += change;
    }

    public IEnumerator ChangeOwner(GameObject newOwner, float delay)
    {
        yield return new WaitForSeconds(delay);
        if(newOwner != GameManager.instance.playerFactionObject)//if AI
        {
            if (settlement == newOwner.GetComponent<AI_Faction>().chosenSettlement)
            {
                if (newOwner != ownerObject)
                {
                    if (newOwner.activeInHierarchy)
                    {
                        if (newOwner.GetComponent<Faction>().enemies.Contains(ownerObject))
                        {
                            if (ownerObject.GetComponent<Faction>().capitalCity == settlement)
                            {
                                print("capital secured");
                                ownerObject.GetComponent<Faction>().Capitulate(newOwner);
                            }
                            else
                            {
                                ownerObject.GetComponent<Faction>().ownedTiles.Remove(gameObject);
                                ownerObject.GetComponent<Faction>().UpdateOwnedTiles();
                                ownerObject.GetComponent<Faction>().UpdateNieghbouringTiles();

                                newOwner.GetComponent<Faction>().AddTile(gameObject);
                                newOwner.GetComponent<Faction>().UpdateOwnedTiles();
                                newOwner.GetComponent<Faction>().UpdateNieghbouringTiles();

                            }
                        }
                        else
                        {
                            print(newOwner.name + "Wasn't fast enough " + ownerObject.name + " got there first");
                        }
                    }
                    else print(newOwner.ToString() + " sadly was too late, and died");
                }
                else
                {
                    print(owner.ToString() + " Tried to kill themselves somehow?");
                }
            }
            else
            {
                print("ERROR -" + newOwner + "- Tried to attack a settlement that wasn't chosen");
            }
            print(newOwner.name + " CAPTURES: " + settlement.name);
            newOwner.GetComponent<AI_Faction>().taskComplete = true;
            newOwner.GetComponent<AI_Faction>().GenerateNextTask();
        }
        else
        {
            if (ownerObject.GetComponent<Faction>().capitalCity == settlement)
            {
                print("capital secured");
                ownerObject.GetComponent<Faction>().Capitulate(newOwner);
            }
            else newOwner.GetComponent<Faction>().AddTile(settlement.transform.parent.gameObject);
        }
        publicOrder -= 30;
        if (culture == newOwner.GetComponent<Faction>().culture) publicOrder += 20;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Tile")
        {
            neighbouringTiles.Add(collision.gameObject);
        }
    }

}
