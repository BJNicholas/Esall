using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settlement : MonoBehaviour
{
    public enum settlementType
    {
        Village,
        City
    }
    public settlementType type;
    public Sprite village, city, capitalCity;
    public string settlementName;
    public Text nameTXT;

    public bool capital = false;
    public GameObject province;
    [Header("Trade")]
    public List<GameObject> merchants;
    public List<Item> storedItems;
    public List<Soldier> availableSoldiers;
    public List<Item> producedItems;
    [Header("Military")]
    public float garrisonSize;

    private void Awake()
    {
        province = transform.parent.gameObject;
        province.GetComponent<Tile>().settlement = gameObject;
        gameObject.name = settlementName;
    }

    private void Start()
    {
        nameTXT.text = settlementName;
        gameObject.name = settlementName;
        SpawnStartingMerchants();
        GenerateStartingGarrison();
    }
    public void GenerateStartingGarrison()
    {
        float startingNum = 2; // base

        startingNum += province.GetComponent<Tile>().development / 2;
        startingNum = Mathf.RoundToInt(startingNum);

        if (capital) startingNum += 1;
        garrisonSize = startingNum;
        
    }
    //AI FUNCTION
    public IEnumerator Muster(GameObject army)
    {
        yield return new WaitForSeconds(0);
        if (army.activeInHierarchy)
        {
            if (availableSoldiers.ToArray().Length > 0)
            {
                float price = availableSoldiers[0].buyPrice;
                if (army.GetComponent<Army>().ownerObject.GetComponent<Faction>().treasury >= price)
                {
                    army.GetComponent<Army>().ownerObject.GetComponent<Faction>().treasury -= price;
                    army.GetComponent<Army>().ownerObject.GetComponent<Faction>().expenses += availableSoldiers[0].cpm;
                    army.GetComponent<Army>().soldiers.Add(Instantiate(availableSoldiers[0]));

                    availableSoldiers.Remove(availableSoldiers[0]);
                }
                else print(army.GetComponent<Army>().owner.ToString() + " Cannot afford soldiers in " + settlementName);
            }
            else print(army.GetComponent<Army>().owner.ToString() + " Cannot recruit any soldiers in " + settlementName + " Because there are none left");

            army.GetComponent<Army>().ownerObject.GetComponent<AI_Faction>().GenerateNextTask();
        }
        else print("Army was destroyed, couldn't complete task");
    }
    //AI FUNCTION
    public IEnumerator Stockpile(GameObject army)
    {
        yield return new WaitForSeconds(0);
        if (army.activeInHierarchy)
        {
            if (storedItems.ToArray().Length > 0)
            {
                Item item = storedItems[Random.Range(0, storedItems.ToArray().Length)];
                float price = item.baseValue;
                if (army.GetComponent<Army>().ownerObject.GetComponent<Faction>().treasury >= price)
                {
                    army.GetComponent<Army>().ownerObject.GetComponent<Faction>().treasury -= price;
                    army.GetComponent<Army>().storedItems.Add(item);

                    storedItems.Remove(item);
                }
                else print(army.GetComponent<Army>().owner.ToString() + " Cannot afford items in " + settlementName);
            }
            else print(army.GetComponent<Army>().owner.ToString() + " Cannot buy any items in " + settlementName + " Because there are none left");

            army.GetComponent<Army>().ownerObject.GetComponent<AI_Faction>().GenerateNextTask();
        }
        else print("Army was destroyed, couldn't complete task");

    }
    //AI FUNCTION
    public IEnumerator Invest(GameObject faction)
    {
        yield return new WaitForSeconds(0);
        if (faction.activeInHierarchy)
        {
            if (faction.GetComponent<Faction>().treasury >= AdministrationHub.instance.FindMerchantPrice(0f, gameObject))
            {
                province.GetComponent<Tile>().publicOrder += 10;
                NewMerchant();
                province.GetComponent<Tile>().ownerObject.GetComponent<Faction>().treasury -= AdministrationHub.instance.FindMerchantPrice(0f, gameObject);
                print(faction.name + " CREATED A MERCHANT IN " + settlementName);
            }
            else print(faction.name + " CAN NOT AFFORD A MERCHANT");
            faction.GetComponent<AI_Faction>().GenerateNextTask();
        }
    }
    //AI FUNCTION
    public IEnumerator GarrisonTroops(GameObject faction)
    {
        yield return new WaitForSeconds(0);
        if (faction.activeInHierarchy)
        {
            if (faction.GetComponent<Faction>().treasury >= AdministrationHub.instance.FindGarrisonPrice(0f, gameObject))
            {
                province.GetComponent<Tile>().publicOrder += 5;
                garrisonSize += 1;
                province.GetComponent<Tile>().ownerObject.GetComponent<Faction>().treasury -= AdministrationHub.instance.FindGarrisonPrice(0f, gameObject);
                print(faction.name + " INCREASED THE GARRISON IN " + settlementName);
            }
            else print(faction.name + " CAN NOT AFFORD A LARGER GARRISON");
            faction.GetComponent<AI_Faction>().GenerateNextTask();
        }
    }
    public void SpawnStartingMerchants()
    {
        if (type == settlementType.City)
        {
            GetComponent<SpriteRenderer>().sprite = city;
            NewMerchant();
        }
    }

    public void NewMerchant()
    {
        GameObject merchant = Instantiate(GameManager.instance.merchantPrefab);
        merchant.transform.position = transform.position;
        merchant.transform.SetParent(null);

        merchant.GetComponent<Merchant>().homeCity = gameObject;
        merchant.GetComponent<Merchant>().owner = province.GetComponent<Tile>().owner;
        merchants.Add(merchant);
        merchant.name = settlementName + " Merchant " + merchants.IndexOf(merchant);
        GameManager.instance.merchants.Add(merchant);
    }
}
