using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitmentHub : MonoBehaviour
{
    public static RecruitmentHub instance;
    [Header("Recruitment details")]
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
            listing.GetComponent<Button>().onClick.AddListener(listing.GetComponent<SoldierUI>().Purchase);
        }
    }

    public void GenerateListings()
    {
        foreach (Soldier soldier in settlement.GetComponent<Settlement>().availableSoldiers)
        {
            if (listings.ToArray().Length > 0)
            {
                bool foundListing = false;
                foreach (GameObject listing in listings) // finding out if item already has a listing
                {
                    if (soldier == listing.GetComponent<SoldierUI>().soldier)
                    {
                        foundListing = true;
                        break;
                    }
                }
                if (foundListing == false)
                {
                    GenerateNewListing(soldier);
                }
            }
            else // make the first Listing
            {
                GameObject listing = Instantiate(itemListingPrefab, scrollViewArea.transform);
                listing.GetComponent<SoldierUI>().soldier = soldier;

                listings.Add(listing);

                foreach (Soldier storedSoldier in settlement.GetComponent<Settlement>().availableSoldiers)
                {
                    if (storedSoldier == listing.GetComponent<SoldierUI>().soldier)
                    {
                        listing.GetComponent<SoldierUI>().amountRemaining += 1;
                    }
                }
            }
        }

        foreach (GameObject listing in listings)
        {

            PriceCheck(listing, listing.GetComponent<SoldierUI>().soldier);
            //positioning
            listing.transform.position += new Vector3(0, -40 * listings.IndexOf(listing));
            //scrollViewArea.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 40);
        }
    }

    public void PriceCheck(GameObject listing, Soldier soldier)
    {
        listing.GetComponent<SoldierUI>().price = soldier.buyPrice;

    }

    public void GenerateNewListing(Soldier newSoldier)
    {
        GameObject listing = Instantiate(itemListingPrefab, scrollViewArea.transform);
        listing.GetComponent<SoldierUI>().soldier = newSoldier;
        listings.Add(listing);

        foreach (Soldier soldier in settlement.GetComponent<Settlement>().availableSoldiers)
        {
            if (soldier == listing.GetComponent<SoldierUI>().soldier)
            {
                listing.GetComponent<SoldierUI>().amountRemaining += 1;
            }
        }

    }

    public void Leave()
    {
        foreach (GameObject listing in listings)
        {
            Destroy(listing);
        }
        listings.Clear();
        gameObject.SetActive(false);
        SettlementInspector.instance.gameObject.SetActive(true);
    }
}
