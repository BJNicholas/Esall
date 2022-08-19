using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiploHub : MonoBehaviour
{
    public static DiploHub instance;
    [Header("stats")]
    public GameObject faction;

    public List<GameObject> decisions;

    [Header("UI Setup")]
    public Text factionName;
    public Image flag;


    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        faction = SettlementInspector.instance.settlement.GetComponent<Settlement>().province.GetComponent<Tile>().ownerObject;
        factionName.text = faction.GetComponent<Faction>().faction.ToString();
        flag.sprite = faction.GetComponent<Faction>().flag;

        SetDecisions();
    }

    public void SetDecisions()
    {
        #region war
        if (!GameManager.instance.playerFactionObject.GetComponent<Faction>().enemies.Contains(faction))
        {
            decisions[0].GetComponentInChildren<Text>().text = "Declare War";
            decisions[0].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            decisions[0].GetComponentInChildren<Button>().onClick.AddListener(DeclareWar);
        }
        else
        {
            decisions[0].GetComponentInChildren<Text>().text = "Propose Peace";
            decisions[0].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            decisions[0].GetComponentInChildren<Button>().onClick.AddListener(ProposePeace);
        }
        #endregion
        #region Alliance
        if (!GameManager.instance.playerFactionObject.GetComponent<Faction>().enemies.Contains(faction))
        {
            if (!GameManager.instance.playerFactionObject.GetComponent<Faction>().allies.Contains(faction))
            {
                decisions[1].GetComponentInChildren<Text>().text = "Request Alliance";
                decisions[1].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                decisions[1].GetComponentInChildren<Button>().onClick.AddListener(RequestAlliance);
            }
            else
            {
                decisions[1].GetComponentInChildren<Text>().text = "Break Alliance";
                decisions[1].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                decisions[1].GetComponentInChildren<Button>().onClick.AddListener(BreakAlliance);
            }
        }
        else
        {
            decisions[1].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        }
        #endregion
    }

    public void DeclareWar()
    {
        GameObject player = GameManager.instance.playerFactionObject;

        if (!player.GetComponent<Faction>().enemies.Contains(faction))
        {
            print("PLAYER DECLARES WAR WITH " + faction.name);

            player.GetComponent<Faction>().atWar = true;
            player.GetComponent<Faction>().enemies.Add(faction);

            faction.GetComponent<Faction>().atWar = true;
            faction.GetComponent<Faction>().enemies.Add(player);
            faction.GetComponent<AI_Faction>().CancelCurrentTask();

            if(faction.GetComponent<Faction>().allies.ToArray().Length > 0)
            {
                foreach (GameObject ally in faction.GetComponent<Faction>().allies)
                {
                    ally.GetComponent<Faction>().atWar = true;
                    player.GetComponent<Faction>().enemies.Add(ally);
                    ally.GetComponent<Faction>().enemies.Add(player);
                    ally.GetComponent<AI_Faction>().CancelCurrentTask();
                    print(ally.name + " CANCELLED BECAUSE A WAR STARTED");
                }
            }

            print(faction.name + " CANCELLED BECAUSE A WAR STARTED");

            GameObject newEvent = Instantiate(GameManager.instance.eventPrefab, GameObject.Find("UI").transform);
            newEvent.GetComponent<Event>().title.text = "WAR";
            newEvent.GetComponent<Event>().description.text = player.name + " has declared war with " + faction.GetComponent<Faction>().faction.ToString();
            newEvent.GetComponent<Event>().dismiss.text = "And we will win!";

            if (faction.GetComponent<Faction>().allies.ToArray().Length > 0)
            {
                newEvent.GetComponent<Event>().description.text += " and their allies: ";
                foreach (GameObject ally in faction.GetComponent<Faction>().allies)
                {
                    newEvent.GetComponent<Event>().description.text += ", " + ally.GetComponent<Faction>().faction.ToString();
                }
            }

            Close(); // close diplo tab
        }
        else
        {
            print("Already at war dumbo");
        }
    }
    public void ProposePeace()
    {
        //this is only TEMP, need to have a peace system later


        GameManager.instance.playerFactionObject.GetComponent<Faction>().enemies.Remove(faction); //remove from player enemies
        faction.GetComponent<Faction>().enemies.Remove(GameManager.instance.playerFactionObject); //remove player from enemies

        //if (faction.GetComponent<Faction>().enemies.ToArray().Length == 0) faction.GetComponent<Faction>().atWar = false;

        if (GameManager.instance.playerFactionObject.GetComponent<Faction>().enemies.ToArray().Length == 0) GameManager.instance.playerFactionObject.GetComponent<Faction>().atWar = false;

        Close();
    }

    public void RequestAlliance()
    {
        GameManager.instance.playerFactionObject.GetComponent<Faction>().allies.Add(faction); //add to player allies
        faction.GetComponent<Faction>().allies.Add(GameManager.instance.playerFactionObject); //add player to allies


        SetDecisions();
    }
    public void BreakAlliance()
    {
        GameManager.instance.playerFactionObject.GetComponent<Faction>().allies.Remove(faction); //remove from player allies
        faction.GetComponent<Faction>().allies.Remove(GameManager.instance.playerFactionObject); //remove player from allies


        SetDecisions();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        SettlementInspector.instance.SetCorrectSettlementType();
    }
}
