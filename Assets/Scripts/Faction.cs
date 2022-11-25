using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.AI;

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

    [Space]
    public GameObject ruler;
    [Space]

    [Header("Faction Stats")]
    public float development;
    public float publicOrder; 
    public List<GameObject> neighbouringTiles;
    public List<NationalModifier> nationalMods;

    [Header("Economics")]
    public float treasury;
    public float expenses;
    public float income;
    [Header("mircos")]
    public float taxIncome;
    public float PO_bonus;
    public float armyWages;
    public float merchantWages;
    [Range(0, 2f)] public float taxRate = 1;
    [Range(1, 2f)] public float armyWageRate = 1.5f;
    [Range(0.1f, 2f)] public float merchantWageRate = 0.5f;
    public int numOfMerchants;
    public float monthlyTrade;

    [Header("Diplomacy")]
    public bool atWar = false;
    public List<GameObject> allies;
    public List<GameObject> enemies;
    public List<Opinion> relations;


    private void Start()
    {
        if(faction != FactionManager.factions.Observer)
        {
            ruler = CharacterManager.instance.CreateNewCharacter(ruler, capitalCity);


            Invoke("UpdateOwnedTiles", 0.01f);
            capitalCity.GetComponent<SpriteRenderer>().sprite = capitalCity.GetComponent<Settlement>().capitalCity;

            SetStartingRelations();
        }
    }

    private void FixedUpdate()
    {
        if (faction != FactionManager.factions.Observer)
        {
            CalculateExpenses();
            CalculateIncome();
            CalculateOrder();


            capitalCity.GetComponent<SpriteRenderer>().sprite = capitalCity.GetComponent<Settlement>().capitalCity;
        }
    }

    public void SetStartingRelations()
    {
        List<GameObject> otherFactions = new List<GameObject>();
        GameObject currentTargetedFaction = null;
        foreach (GameObject faction in FactionManager.instance.factionObjects)
        {
            if (faction != gameObject)
            {
                otherFactions.Add(faction);
                Opinion newRelation = new Opinion();
                newRelation.faction = faction.GetComponent<Faction>().faction;
                newRelation.factionObj = faction;
                currentTargetedFaction = faction;
                relations.Add(newRelation);
            }
        }
        foreach (Opinion relation in relations)
        {
            relation.opinion = Random.Range(20, 70);
            //filling out starting Modifiers
            if (currentTargetedFaction.GetComponent<Faction>().culture != culture)
            {
                relation.modifiers.Add(Instantiate(GameManager.instance.modifiers[0])); //Cultural Differences
            }
            else
            {
                relation.modifiers.Add(Instantiate(GameManager.instance.modifiers[1])); //Cultural Understanding
            }
        }
        //AdjustRelations();
    }

    public void AdjustRelations()
    {
        // for each faction 
        foreach(Opinion relation in relations)
        {
            relation.AdjustOpinion();
        }
    }

    public void CommittedAct(Modifier mod)
    {
        foreach (GameObject faction in FactionManager.instance.factionObjects)
        {
            if (faction != gameObject)
            {
                foreach (Opinion relation in faction.GetComponent<Faction>().relations)
                {
                    if(relation.faction == gameObject.GetComponent<Faction>().faction)
                    {
                        relation.modifiers.Add(Instantiate(mod));
                    }
                }
            }
        }
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
        if (gameObject != GameManager.instance.playerFactionObject)
        {
            GetComponent<AI_Faction>().StopAllCoroutines();
            GetComponent<AI_Faction>().enabled = false;
            DiplomacyTab.instance.DestroyDeadFactionFromUI(gameObject);

            //player wins
            if(DiplomacyTab.instance.allRelationUI_Objects.ToArray().Length == 0)
            {
                //playerr dies and lose screen is activated
                GameManager.instance.win.SetActive(true);
                Time.timeScale = 1f;
            }
        }
        else
        {
            //playerr dies and lose screen is activated
            SettlementInspector.instance.CloseAllDown();
            GameManager.instance.lose.SetActive(true);
            Time.timeScale = 1f;
            Camera.main.GetComponent<CameraController>().target = GameManager.instance.gameObject;
        }

        if (army != null) Destroy(army);
        int tilesLeft = ownedTiles.ToArray().Length - 1;
        capitalCity.GetComponent<SpriteRenderer>().sprite = capitalCity.GetComponent<Settlement>().city;
        capitalCity.GetComponent<Settlement>().capital = false;
        while (tilesLeft != 0 - 1)
        {
            victoriousFaction.GetComponent<Faction>().AddTile(ownedTiles[tilesLeft]);
            tilesLeft -= 1;
        }
        atWar = false;
        foreach (GameObject faction in enemies)
        {
            faction.GetComponent<Faction>().enemies.Remove(gameObject);
        }

        print(faction.ToString() + " Has Capitulated to " + victoriousFaction);
        Console.instance.PrintMessage(faction.ToString() + " Has Capitulated to " + victoriousFaction, Color.magenta);
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

                army.GetComponent<Army>().soldiers.Add(Instantiate(army.GetComponent<Army>().soldiers[0]));
                army.GetComponent<Army>().soldiers.RemoveAt(0);

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

    public void CalculateIncome()
    {
        income = 0;

        taxIncome = 0;
        PO_bonus = 0;

        //Tax
        foreach(GameObject tile in ownedTiles)
        {
            tile.GetComponent<Tile>().taxIncome = 0;

            tile.GetComponent<Tile>().TaxIncomeCalculation();

            tile.GetComponent<Tile>().taxIncome *= taxRate;
            taxIncome += tile.GetComponent<Tile>().taxIncome;
        }

        //PO Bonus
        if(publicOrder >= 80)
        {
            PO_bonus += (publicOrder - 80) * (development / 20);
        }
        else
        {
            PO_bonus -= 5;
            PO_bonus -= (80 - publicOrder) * (development / 20);
        }

        //toatls
        income += taxIncome;
        income += PO_bonus;
    }

    public void CalculateExpenses()
    {
        //reset vars
        expenses = 0;
        armyWages = 0;
        merchantWages = 0;


        //army
        if(army != null)
        {
            foreach (Soldier soldier in army.GetComponent<Army>().soldiers)
            {
                armyWages += soldier.cpm;
            }
            armyWages *= armyWageRate;
        }
        else armyWages = 0;

        //merchants
        merchantWages += numOfMerchants * merchantWageRate;


        //total
        expenses += armyWages;
        expenses += merchantWages;

    }
    public void CalculateOrder()
    {
        publicOrder = 0;
        float combinedOrder = 0;
        foreach (GameObject tile in ownedTiles)
        {
            combinedOrder += tile.GetComponent<Tile>().publicOrder;
        }
        publicOrder = (combinedOrder / (ownedTiles.ToArray().Length * 100)) * 100;
    }
    public void ChargeForIncome()
    {
        treasury += income;
    }

    public void PayExpenses()
    {
        treasury -= expenses;

        #region functions of expenses

        //army.GetComponent<NavMeshAgent>().speed = 0.1f * armyWageRate;

        //merchant speed affected
        foreach (GameObject merchant in GameManager.instance.merchants)
        {
            merchant.GetComponent<Merchant>().speed = 0.3f; // the default
            if(merchant.GetComponent<Merchant>().owner == faction)
            {
                merchant.GetComponent<Merchant>().speed *= merchantWageRate;
            }
        }

        #endregion
    }
}
