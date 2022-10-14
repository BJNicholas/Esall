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

    [HideInInspector]public float resetTimer = 0;
    public void ResetAI()
    {
        resetTimer = 0;
        print(ownerObject.ToString() + " Is resetting");
        ownerObject.GetComponent<AI_Faction>().CancelCurrentTask();
    }

    private void Update()
    {
        army.GetComponent<Army>().stateTXT.text = ownerObject.GetComponent<AI_Faction>().currentTask.ToString();

        Vector2 target, current;
        target = army.GetComponent<Army>().target;
        current = army.transform.position;
        if (target == current) resetTimer++;
        else resetTimer = 0;
        if (resetTimer >= 5000) ResetAI();
        
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
