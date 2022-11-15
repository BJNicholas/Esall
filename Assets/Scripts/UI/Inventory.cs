using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public GameObject army;
    public GameObject scrollViewArea;
    public GameObject itemListingPrefab;
    public List<GameObject> listings;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        GenerateListings();
        LiveUpdateListings();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) CloseMenu();
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
        GenerateListings();

        foreach (GameObject listing in listings)
        {
            //set action
            listing.GetComponent<Button>().onClick.RemoveAllListeners();
            listing.GetComponent<Button>().onClick.AddListener(listing.GetComponent<ItemUI>().Sell);
            listing.name = listing.GetComponent<ItemUI>().item.ToString();
            PriceCheck(listing, listing.GetComponent<ItemUI>().item);
        }
    }

    public void GenerateListings()
    {
        //army.GetComponent<Army>().storedItems.Sort();
        foreach (Item item in army.GetComponent<Army>().storedItems)
        {
            if (listings.ToArray().Length > 0)
            {
                bool foundListing = false;
                foreach (GameObject listing in listings) // finding out if item already has a listing
                {
                    if (item == listing.GetComponent<ItemUI>().item)
                    {
                        foundListing = true;
                        break;
                    }
                }
                if (foundListing == false)
                {
                    GenerateNewListing(item);
                }
            }
            else // make the first Listing
            {
                GameObject listing = Instantiate(itemListingPrefab, scrollViewArea.transform);
                listing.GetComponent<ItemUI>().item = item;

                listings.Add(listing);

                foreach (Item storedItem in army.GetComponent<Army>().storedItems)
                {
                    if (storedItem == listing.GetComponent<ItemUI>().item)
                    {
                        listing.GetComponent<ItemUI>().amountRemaining += 1;
                    }
                }
            }
        }
    }

    public void GenerateNewListing(Item newItem)
    {
        GameObject listing = Instantiate(itemListingPrefab, scrollViewArea.transform);
        listing.GetComponent<ItemUI>().item = newItem;
        listings.Add(listing);

        foreach (Item item in army.GetComponent<Army>().storedItems)
        {
            if (item == listing.GetComponent<ItemUI>().item)
            {
                listing.GetComponent<ItemUI>().amountRemaining += 1;
            }
        }

    }

    public void PriceCheck(GameObject listing, Item item)
    {
       listing.GetComponent<ItemUI>().price = item.baseValue;

        if (SettlementInspector.instance.gameObject.activeInHierarchy == false)
        {
            listing.GetComponent<ItemUI>().price = item.baseValue;
        }
        else
        {
            if (SettlementInspector.instance.settlement.GetComponent<Settlement>().storedItems.Contains(item))
            {
                print("Market already contains: " + item.name);
                GameObject matchingListing = null;

          
                foreach (GameObject listedItem in TradingHub.instance.listings)
                {
                    if (listedItem.GetComponent<ItemUI>().item == listing.GetComponent<ItemUI>().item)
                    {
                        matchingListing = listedItem;
                        break;
                    }
                }

                print(item + " is being matched against: " + matchingListing.name);
                listing.GetComponent<ItemUI>().price = matchingListing.GetComponent<ItemUI>().price;
            }
            else
            {
                print("Market does not contain: " + item.name);
                listing.GetComponent<ItemUI>().price = item.baseValue +( 0.1f * 10);
            }
        }
    }

    public void CloseMenu()
    {
        foreach (GameObject listing in listings)
        {
            Destroy(listing);
        }
        listings.Clear();
        gameObject.SetActive(false);
    }
}
