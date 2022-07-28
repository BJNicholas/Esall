using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiploHub : MonoBehaviour
{
    [Header("stats")]
    public GameObject faction;

    [Header("UI Setup")]
    public Text factionName;
    public Image flag;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        faction = SettlementInspector.instance.settlement.GetComponent<Settlement>().province.GetComponent<Tile>().ownerObject;
        factionName.text = faction.GetComponent<Faction>().faction.ToString();
        flag.sprite = faction.GetComponent<Faction>().flag;
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
            print(faction.name + " CANCELLED BECAUSE A WAR STARTED");

            GameObject newEvent = Instantiate(GameManager.instance.eventPrefab, GameObject.Find("UI").transform);
            newEvent.GetComponent<Event>().title.text = "WAR";
            newEvent.GetComponent<Event>().description.text = player.name + " has declared war with " + faction.GetComponent<Faction>().faction.ToString();
            newEvent.GetComponent<Event>().dismiss.text = "And we will win!";

            Close(); // close diplo tab
        }
        else
        {
            print("Already at war dumbo");
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
        SettlementInspector.instance.SetCorrectSettlementType();
    }
}
