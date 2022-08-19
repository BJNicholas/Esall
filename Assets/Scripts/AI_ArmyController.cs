using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_ArmyController : MonoBehaviour
{
    GameObject ownerObject;
    GameObject army;

    private void Start()
    {
        army = gameObject;
        foreach(GameObject faction in FactionManager.instance.factionObjects)
        {
            if(faction.GetComponent<Faction>().faction == army.GetComponent<Army>().owner)
            {
                ownerObject = faction;
            }
        }

        ownerObject.GetComponent<AI_Faction>().CreatePersonality();
        ownerObject.GetComponent<AI_Faction>().GenerateNextTask();
    }

    private void Update()
    {
        army.GetComponent<Army>().stateTXT.text = ownerObject.GetComponent<AI_Faction>().currentTask.ToString();
    }

    void Idle()
    {
        ////1 province minors default to defending
        //if(ownerObject.GetComponent<Faction>().ownedTiles.ToArray().Length == 1)
        //{
        //    currentState = Army.possibleStates.Defending;
        //    Defending();
        //}
        ////bigger factions..
        //else
        //{
        //    //defaults to patrolling 
        //    currentState = Army.possibleStates.Patrolling;
        //    Patrolling();
        //}
    }
    void Defending()
    {
        army.GetComponent<Army>().target = ownerObject.GetComponent<Faction>().capitalCity.transform.position;
    }
    void Patrolling()
    {
        int roll = Random.Range(0, ownerObject.GetComponent<Faction>().ownedTiles.ToArray().Length);
        army.GetComponent<Army>().target = ownerObject.GetComponent<Faction>().ownedTiles[roll].GetComponent<Tile>().settlement.transform.position;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject == ownerObject.GetComponent<AI_Faction>().chosenSettlement && ownerObject.GetComponent<AI_Faction>().arrived == false)
        {
            ownerObject.GetComponent<AI_Faction>().arrived = true;
        }
    }
}
