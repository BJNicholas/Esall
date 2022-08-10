using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Siege : MonoBehaviour
{
    public float timeRemaining;
    GameObject army;


    [Header("UI SET UP")]
    public Image fill;
    float fillTickAmount = 0;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Encounter.instance.gameObject.activeInHierarchy)
        {
            Abandon();
        }
    }


    public void StartSiege()
    {
        army = GameManager.instance.playerFactionObject.GetComponent<Faction>().army;
        //reseting UI
        fill.fillAmount = 0;

        timeRemaining = SettlementInspector.instance.settlement.GetComponent<Settlement>().garrisonSize * 3; //base time
        timeRemaining -= army.GetComponent<Army>().numOfSoldiers;

        fillTickAmount = 1 / timeRemaining;

        StartCoroutine(SiegeTick());
    }

    public IEnumerator SiegeTick()
    {
        yield return new WaitForSeconds(1);
        timeRemaining -= 1;
        //Update UI
        fill.fillAmount += fillTickAmount;

        if (timeRemaining <= 0)
        {
            //siege won
            SettlementInspector.instance.TakeProvince();
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(SiegeTick());
        }
    }


    public void Abandon()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}
