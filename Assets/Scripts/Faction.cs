using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Faction : MonoBehaviour
{
    [Header("Setup")]
    public int id;
    public FactionManager.factions faction;
    public CultureManager.cultures culture;
    public Color32 colour;
    public Sprite flag;
    public GameObject factionTxtObject;
    public Text factionTxt;
    [Space]

    public List<GameObject> ownedTiles;

    public GameObject capitalCity;
    public GameObject army;

    [Header("Faction Stats")]
    public float development;
    public List<GameObject> neighbouringTiles;
    [Header("Economics")]
    public float treasury;
    public float taxIncome;
    public float expenses = 0;

    [Header("Diplomacy")]
    public bool atWar = false;
    public List<GameObject> allies;
    public List<GameObject> enemies;


    private void Start()
    {
        if(faction != FactionManager.factions.Observer)
        {
            Invoke("UpdateOwnedTiles", 0.01f);
        }
    }

    private void FixedUpdate()
    {
        CalculateTax();
    }
    public void AddTile(GameObject newTile)
    {
        //remove Tile from previous owner;
        newTile.GetComponent<Tile>().ownerObject.GetComponent<Faction>().ownedTiles.Remove(newTile);
        newTile.GetComponent<Tile>().ownerObject.GetComponent<Faction>().UpdateOwnedTiles();
        newTile.GetComponent<Tile>().ownerObject.GetComponent<Faction>().GenerateNamePlacement();
        newTile.GetComponent<Tile>().owner = faction;
        newTile.GetComponent<Tile>().ownerObject = gameObject;
        ownedTiles.Add(newTile);
        //if the province has a city
        if (newTile.GetComponent<Tile>().settlement.GetComponent<Settlement>().type == Settlement.settlementType.City)
        {
            foreach (GameObject merchant in newTile.GetComponent<Tile>().settlement.GetComponent<Settlement>().merchants)
            {
                merchant.GetComponent<Merchant>().UpdateOwner(faction);
            }
        }
        UpdateOwnedTiles();
    }


    public void UpdateOwnedTiles()
    {
        development = 0;
        foreach (GameObject tile in ownedTiles)
        {
            //adding to the total dev of the faction
            development += tile.GetComponent<Tile>().development;
        }
        GenerateNamePlacement(); // name placement
        UpdateNieghbouringTiles();
    }
    public void UpdateNieghbouringTiles()
    {
        neighbouringTiles.Clear();
        List<GameObject> foriegnTiles = new List<GameObject>();
        foreach (GameObject tile in ownedTiles)
        {
            foreach (GameObject neighbour in tile.GetComponent<Tile>().neighbouringTiles)
            {
                if(neighbour.GetComponent<Tile>().owner != faction)
                {
                    neighbouringTiles.Add(neighbour);
                }
            }
        }
        neighbouringTiles = neighbouringTiles.Distinct().ToList();
    }

    public void Capitulate(GameObject victoriousFaction)
    {
        if (army != null) Destroy(army);
        int tilesLeft = ownedTiles.ToArray().Length -1;
        while(tilesLeft != 0 -1)
        {
            victoriousFaction.GetComponent<Faction>().AddTile(ownedTiles[tilesLeft]);
            tilesLeft -= 1;
        }
        atWar = false;
        foreach(GameObject faction in enemies)
        {
            faction.GetComponent<Faction>().enemies.Remove(gameObject);
        }

        print(faction.ToString() + " Has Capitulated to " + victoriousFaction);
    }

    public void GenerateNamePlacement()
    {
        factionTxt.text = faction.ToString();
        //gettting the furthest provinces
        if (ownedTiles.ToArray().Length > 1)
        {
            float maxDistance = 0;
            Vector3 pointA = Vector3.zero, pointB = Vector3.zero;
            Vector2 midPoint = Vector2.zero;
            foreach (GameObject tile in ownedTiles)
            {
                foreach (GameObject otherTile in ownedTiles)
                {
                    float distance = Vector2.Distance(tile.transform.position, otherTile.transform.position);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        midPoint = (tile.transform.position + otherTile.transform.position) / 2;
                        if (tile.transform.position.x > otherTile.transform.position.x)
                        {
                            pointA = tile.transform.position;
                            pointB = otherTile.transform.position;
                        }
                        else
                        {
                            pointB = tile.transform.position;
                            pointA = otherTile.transform.position;
                        }
                    }
                }
            }
            //angle
            Vector3 vectorToTarget = pointA - pointB;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            factionTxtObject.gameObject.transform.rotation = q;
            //placement and size
            factionTxtObject.gameObject.transform.position = midPoint;
            factionTxtObject.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxDistance);
        }
        else if (ownedTiles.ToArray().Length == 1) //only one tile
        {
            //placement
            factionTxtObject.gameObject.transform.position = ownedTiles[0].transform.position;
            factionTxtObject.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1);
        }
        else if (ownedTiles.ToArray().Length == 0) //no tiles left
        {
            gameObject.SetActive(false);
        }

    }

    public void SpawnNewArmy()
    {
        if(army == null)
        {
            if (gameObject.activeInHierarchy)
            {
                GameObject newArmy = Instantiate(GameManager.instance.armyPrefab, null);
                newArmy.name = faction.ToString() + " Army";
                newArmy.transform.position = capitalCity.transform.position;
                newArmy.GetComponent<Army>().owner = faction;
                newArmy.GetComponent<Army>().ownerObject = gameObject;
                newArmy.GetComponent<SpriteRenderer>().color = new Color32(colour.r, colour.g, colour.b, 200);

                newArmy.GetComponent<Army>().currentProvince = capitalCity.GetComponent<Settlement>().province;

                army = newArmy;

                if (faction == GameManager.instance.playerFaction)
                {
                    newArmy.GetComponent<Army>().isPlayer = true;
                    Camera.main.GetComponent<CameraController>().FindPlayerArmy();
                }
            }
            else print("FACTION DIED, no army spawn");
        }
        else
        {
            if (gameObject.activeInHierarchy)
            {
                army.transform.position = capitalCity.transform.position;
                army.SetActive(true);
            }
            else print("FACTION DIED, no army spawn");
        }
    }

    public void CalculateTax()
    {
        taxIncome = 0;
        foreach(GameObject tile in ownedTiles)
        {
            taxIncome += tile.GetComponent<Tile>().taxIncome;
        }
    }
    public void chargeTax()
    {
        treasury += taxIncome;
        //print(faction.ToString() + " taxable income = " + development / 10);
    }

    public void PayExpenses()
    {
        treasury -= expenses;
    }
}
