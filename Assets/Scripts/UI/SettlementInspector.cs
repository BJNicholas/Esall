using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettlementInspector : MonoBehaviour
{
    public static SettlementInspector instance;
    public GameObject settlement;
    [Header("UI setup")]
    public Text settlementName, factionName;
    public Image flag;
    public Text development, garrison, merchants;
    [Header("Sub Panels")]
    public GameObject foreign;
    public GameObject enemy;
    public GameObject owned;
    public GameObject capital;

    [Header("Non-static Tabs")]
    public GameObject administerHub;
    public GameObject diploHub;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }


    public void SetCorrectSettlementType()
    {
        if (settlement.GetComponent<Settlement>().province.GetComponent<Tile>().ownerObject == GameManager.instance.playerFactionObject) // owned by player
        {
            capital.SetActive(false);
            foreign.SetActive(false);
            enemy.SetActive(false);
            owned.SetActive(true);
            if (settlement == GameManager.instance.playerFactionObject.GetComponent<Faction>().capitalCity)// is capital
            {
                capital.SetActive(true);
                foreign.SetActive(false);
                enemy.SetActive(false);
                owned.SetActive(false);
            }
        }
        else //foreign
        {
            if (GameManager.instance.playerFactionObject.GetComponent<Faction>().enemies.Contains(settlement.GetComponent<Settlement>().province.GetComponent<Tile>().ownerObject))//enemy
            {
                capital.SetActive(false);
                foreign.SetActive(false);
                enemy.SetActive(true);
                owned.SetActive(false);
            }
            else
            {
                capital.SetActive(false);
                foreign.SetActive(true);
                enemy.SetActive(false);
                owned.SetActive(false);
            }
        }
    }

    void ReloadOwner()
    {
        //title INFO
        settlementName.text = settlement.GetComponent<Settlement>().settlementName;
        factionName.text = settlement.GetComponent<Settlement>().province.GetComponent<Tile>().owner.ToString();
        flag.sprite = settlement.GetComponent<Settlement>().province.GetComponent<Tile>().ownerObject.GetComponent<Faction>().flag;
    }

    private void Update()
    {
        ReloadOwner();
        development.text = settlement.GetComponent<Settlement>().province.GetComponent<Tile>().development.ToString();
        merchants.text = settlement.GetComponent<Settlement>().merchants.ToArray().Length.ToString();
        garrison.text = settlement.GetComponent<Settlement>().garrisonSize.ToString();

        SetCorrectSettlementType();
    }

    public void Attack()
    {
        Encounter.instance.gameObject.SetActive(true);

        Encounter.instance.otherfaction = settlement.GetComponent<Settlement>().province.GetComponent<Tile>().ownerObject;
        GameObject garrisonArmy = Instantiate(GameManager.instance.armyPrefab, settlement.transform);
        Encounter.instance.otherArmy = garrisonArmy;
        //setup
        garrisonArmy.name = settlement.name + " Garrison";
        garrisonArmy.GetComponent<Army>().isGarrison = true;
        garrisonArmy.GetComponent<Army>().owner = settlement.GetComponent<Settlement>().province.GetComponent<Tile>().owner;
        garrisonArmy.GetComponent<Army>().ownerObject = Encounter.instance.otherfaction;
        garrisonArmy.GetComponent<Army>().currentProvince = settlement.GetComponent<Settlement>().province;
        //soldiers setup
        while(garrisonArmy.GetComponent<Army>().soldiers.ToArray().Length < settlement.GetComponent<Settlement>().garrisonSize)
        {
            garrisonArmy.GetComponent<Army>().soldiers.Add(garrisonArmy.GetComponent<Army>().soldiers[0]);
        }
        //positioning
        garrisonArmy.transform.position = settlement.transform.position;
        //colouring
        Color32 colour = Encounter.instance.otherfaction.GetComponent<Faction>().colour;
        garrisonArmy.GetComponent<SpriteRenderer>().color = new Color32(colour.r, colour.g, colour.b, 200);


        Encounter.instance.dialogue += settlement.name + " will never fall!";
    }

    public void TakeProvince()
    {
        if (settlement.transform.parent.gameObject.GetComponent<Tile>().ownerObject.GetComponent<Faction>().capitalCity == settlement.transform.parent.gameObject.GetComponent<Tile>().settlement)
        {
            print("capital secured");
            settlement.transform.parent.gameObject.GetComponent<Tile>().ownerObject.GetComponent<Faction>().Capitulate(GameManager.instance.playerFactionObject);
        }
        else GameManager.instance.playerFactionObject.GetComponent<Faction>().AddTile(settlement.transform.parent.gameObject);

        SetCorrectSettlementType();
    }

    public void Trade()
    {
        TradingHub.instance.gameObject.SetActive(true);
        TradingHub.instance.settlement = settlement;
        TradingHub.instance.GenerateListings();
        //gameObject.SetActive(false);
    }
    public void Recruit()
    {
        RecruitmentHub.instance.gameObject.SetActive(true);
        RecruitmentHub.instance.settlement = settlement;
        RecruitmentHub.instance.GenerateListings();
        //gameObject.SetActive(false);
    }
    public void Administer()
    {
        administerHub.SetActive(true);
    }
    public void Diplomacy()
    {
        diploHub.SetActive(true);
    }

    public void Leave()
    {
        settlement = null;
        capital.SetActive(false);
        foreign.SetActive(false);
        enemy.SetActive(false);
        owned.SetActive(false);
        gameObject.SetActive(false);
    }
}
