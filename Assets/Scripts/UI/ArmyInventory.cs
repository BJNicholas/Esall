using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ArmyInventory : MonoBehaviour
{
    public static ArmyInventory instance;
    public GameObject army;
    public GameObject armyViewArea;
    public GameObject unitListingPrefab;
    public List<GameObject> listings;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        army = GameManager.instance.playerFactionObject.GetComponent<Faction>().army;
        GenerateListings();
        LiveUpdateListings();
    }

    public void LiveUpdateListings()
    {
        //clear old 
        foreach (GameObject listing in listings)
        {
            Destroy(listing);
        }
        listings.Clear();
        //re run for new
        if (GameManager.instance.playerFactionObject.GetComponent<Faction>().army != null) 
        {
            army = GameManager.instance.playerFactionObject.GetComponent<Faction>().army;
            GenerateListings(); 
        }
        else
        {
            print("No soldiers becuase army is dead");
        }

    }

    public void GenerateListings()
    {
        foreach (Soldier soldier in army.GetComponent<Army>().soldiers)
        {
            GenerateNewListing(soldier);
        }
    }

    public void GenerateNewListing(Soldier soldier)
    {
        GameObject listing = Instantiate(unitListingPrefab, armyViewArea.transform);
        listing.GetComponent<SoldierInventoryUI>().soldier = soldier;
        listings.Add(listing);
        armyViewArea.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 100);

    }

    public void CloseMenu()
    {
        foreach (GameObject listing in listings)
        {
            Destroy(listing);
        }
        listings.Clear();
        armyViewArea.GetComponent<RectTransform>().sizeDelta = new Vector2(0.8134586f, 100);
    }
}
