using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    GameObject army;

    private void Start()
    {
        army = gameObject;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && SettlementInspector.instance.gameObject.activeInHierarchy == false)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.tag == "UI")
                {
                    print("Clicked On UI");
                }
                else if (hit.collider.tag == "Map")
                {
                    army.GetComponent<Army>().target = hit.point;
                }
                else if (hit.collider.tag == "Settlement")
                {
                    print("Going to Settlement");
                    army.GetComponent<Army>().target = hit.collider.gameObject.transform.position;
                }
                else if (hit.collider.tag == "Army")
                {
                    print("Moving to attack");
                    army.GetComponent<Army>().target = hit.collider.gameObject.transform.position;
                }
                else if (hit.collider.tag == "Merchant")
                {
                    print("Intercepting Merchant");
                    army.GetComponent<Army>().target = hit.collider.gameObject.transform.position;
                }
                else
                {
                    print("CANT GO THERE");
                }
            }
            else print("CANT GO THERE");
        }

        if (Encounter.instance.gameObject.activeInHierarchy) army.GetComponent<Army>().target = army.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Army" && collision.gameObject.GetComponent<Army>().isGarrison == false)
        {
            Encounter.instance.gameObject.SetActive(true);
            //Encounter.instance.settlement = collision.gameObject;
            army.GetComponent<Army>().target = collision.gameObject.transform.position;

            Encounter.instance.otherfaction = collision.gameObject.GetComponent<Army>().ownerObject;
            Encounter.instance.otherArmy = collision.gameObject;

            Encounter.instance.otherfaction.GetComponent<AI_Faction>().currentTask = AI_Faction.PossibleTasks.Encounter;
            Encounter.instance.otherfaction.GetComponent<AI_Faction>().CancelCurrentTask();

            Encounter.instance.dialogue += "What does the King of " + GameManager.instance.playerFaction.ToString() + " want from me? ";
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Settlement" && collision.gameObject.transform.position == army.GetComponent<Army>().target)
        {
            SettlementInspector.instance.gameObject.SetActive(true);
            SettlementInspector.instance.settlement = collision.gameObject;
            army.GetComponent<Army>().target = collision.gameObject.transform.position;

            SettlementInspector.instance.SetCorrectSettlementType();
        }
    }
}
