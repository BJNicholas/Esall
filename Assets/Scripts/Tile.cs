using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [HideInInspector] public int id;
    public FactionManager.factions owner;
    public GameObject settlement;
    public GameObject ownerObject;
    public List<GameObject> neighbouringTiles;

    [Header("Tile Stats")]
    [Range(0,10)]public int development;
    public bool coastal;

    private void Start()
    {
        foreach(GameObject faction in FactionManager.instance.factionObjects)
        {
            if(faction.GetComponent<Faction>().faction == owner)
            {
                ownerObject = faction;
                ownerObject.GetComponent<Faction>().ownedTiles.Add(gameObject);
            }
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
            ownerObject.GetComponent<Faction>().UpdateOwnedTiles();
        }
        else
        {
            //not enough money
            print(owner.ToString() + " cannot afford to develop " + settlement.GetComponent<Settlement>().settlementName);
        }
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Tile")
        {
            neighbouringTiles.Add(collision.gameObject);
        }
    }

}
