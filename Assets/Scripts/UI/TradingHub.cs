using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradingHub : MonoBehaviour
{
    public static TradingHub instance;
    [Header("Trading stats")]
    public GameObject settlement;
    public GameObject scrollViewArea;
    public GameObject itemListingPrefab;
    public List<GameObject> listings;

    [Header("UI Setup")]
    public Text settlementName;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        settlement = SettlementInspector.instance.settlement;
        settlementName.text = settlement.GetComponent<Settlement>().settlementName;
        LiveUpdateListings();
    }

    private void Update()
    {
        foreach(GameObject item in listings)
        {
            print(item.name + " listing is selling: " + item.GetComponent<ItemUI>().item);
        }
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
        //settlement.GetComponent<Settlement>().storedItems.Sort();
        GenerateListings();

        foreach (GameObject listing in listings)
        {
            //set action
            listing.GetComponent<Button>().onClick.RemoveAllListeners();
            listing.GetComponent<Button>().onClick.AddListener(listing.GetComponent<ItemUI>().Purchase);
        }
    }

    public void GenerateListings()
    {
        foreach (Item item in settlement.GetComponent<Settlement>().storedItems)
        {
            if(listings.ToArray().Length > 0)
            {
                bool foundListing = false;
                foreach (GameObject listing in listings) // finding out if item already has a listing
                {
                    if(item == listing.GetComponent<ItemUI>().item)
                    {
                        foundListing = true;
                        break;
                    }
                }
                if(foundListing == false)
                {
                    GenerateNewListing(item);
                }
            }
            else // make the first Listing
            {
                GameObject listing = Instantiate(itemListingPrefab, scrollViewArea.transform);
                listing.GetComponent<ItemUI>().item = item;
                
                listings.Add(listing);

                foreach (Item storedItem in settlement.GetComponent<Settlement>().storedItems)
                {
                    if (storedItem == listing.GetComponent<ItemUI>().item)
                    {
                        listing.GetComponent<ItemUI>().amountRemaining += 1;
                    }
                }
            }
        }

        foreach(GameObject listing in listings)
        {

            PriceCheck(listing, listing.GetComponent<ItemUI>().item);
            //positioning
            listing.transform.position += new Vector3(0, -40 * listings.IndexOf(listing));
            //scrollViewArea.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 40);
        }
    }

    public void PriceCheck(GameObject listing, Item item)
    {
        listing.GetComponent<ItemUI>().price = item.baseValue + 0.1f * (10 - listing.GetComponent<ItemUI>().amountRemaining);
        if (listing.GetComponent<ItemUI>().price <= item.baseValue) listing.GetComponent<ItemUI>().price = item.baseValue;
    }

    public void GenerateNewListing(Item newItem)
    {
        GameObject listing = Instantiate(itemListingPrefab, scrollViewArea.transform);
        listing.GetComponent<ItemUI>().item = newItem;
        listings.Add(listing);

        foreach (Item item in settlement.GetComponent<Settlement>().storedItems)
        {
            if (item == listing.GetComponent<ItemUI>().item)
            {
                listing.GetComponent<ItemUI>().amountRemaining += 1;
            }
        }

    }

    public void Leave()
    {
        foreach(GameObject listing in listings)
        {
            Destroy(listing);
        }
        listings.Clear();
        gameObject.SetActive(false);
        SettlementInspector.instance.gameObject.SetActive(true);
    }
}
