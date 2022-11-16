using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFirer : MonoBehaviour
{
    public static EventFirer instance;

    public float eventChance = 25;

    public PossibleEvent[] allPossibleEvents;

    private void Awake()
    {
        instance = this;
    }


    public void FireEvent(GameObject affectedFaction)
    {
        float roll = Random.Range(0, 100);
        if(roll <= eventChance)// fire event
        {
            int roll2 = Random.Range(0, allPossibleEvents.Length);
            PossibleEvent chosenEvent = Instantiate(allPossibleEvents[roll2]);

            GameObject eventRefference = null;

            if(affectedFaction == GameManager.instance.playerFactionObject) //create UI for the player only
            {
                GameObject newEvent = Instantiate(GameManager.instance.eventPrefab, GameObject.Find("UI").transform);
                FillEventDetails(newEvent, chosenEvent);

                eventRefference = newEvent;
            }

            NationalModifier uniqueMod = Instantiate(chosenEvent.modifier);
            chosenEvent.modifier = uniqueMod;
            int roll3 = Random.Range(0, affectedFaction.GetComponent<Faction>().ownedTiles.ToArray().Length);
            uniqueMod.affectedArea = affectedFaction.GetComponent<Faction>().ownedTiles[roll3].GetComponent<Tile>().settlement;

            affectedFaction.GetComponent<Faction>().nationalMods.Add(uniqueMod);

            //eventRefference.GetComponent<Event>().PE.EventSetUp(eventRefference);

            print("EVENT for " + affectedFaction.name + ", " + chosenEvent.title);
        }
        else print("No event this month for " + affectedFaction.name + ", roll = " + roll);
    }


    public void FillEventDetails(GameObject eventUI,PossibleEvent chosenEvent)
    {
        eventUI.GetComponent<Event>().PE = chosenEvent;
        eventUI.GetComponent<Event>().title.text = chosenEvent.title;
        eventUI.GetComponent<Event>().description.text = chosenEvent.description;
        eventUI.GetComponent<Event>().dismiss.text = chosenEvent.dismissText;
        eventUI.GetComponent<Event>().image.sprite = chosenEvent.image;
    }
}
