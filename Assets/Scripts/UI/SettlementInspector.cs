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
    public Text development, garrison, merchants, culture, income;
    [Header("Sub Panels")]
    public GameObject foreign;
    public GameObject enemy;
    public GameObject owned;
    public GameObject capital;
    public GameObject siege;

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
        culture.text = settlement.GetComponent<Settlement>().province.GetComponent<Tile>().culture.ToString();
        if(culture.text == settlement.GetComponent<Settlement>().province.GetComponent<Tile>().ownerObject.GetComponent<Faction>().culture.ToString())
        {
            culture.color = Color.green;
        }
        else
        {
            culture.color = Color.red;
        }
        income.text = settlement.GetComponent<Settlement>().province.GetComponent<Tile>().taxIncome.ToString();

        SetCorrectSettlementType();
    }

    public void Attack()
    {
        siege.SetActive(true);
        siege.GetComponent<Siege>().StartSiege();
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
        TradingHub.instance.LiveUpdateListings();
        //gameObject.SetActive(false);
    }
    public void Recruit()
    {
        RecruitmentHub.instance.gameObject.SetActive(true);
        RecruitmentHub.instance.settlement = settlement;
        RecruitmentHub.instance.GenerateListings();
        RecruitmentHub.instance.LiveUpdateListings();
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
