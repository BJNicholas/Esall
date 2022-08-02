using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Encounter : MonoBehaviour
{
    public static Encounter instance;

    public string dialogue;
    [Header("Management")]
    public GameObject otherfaction;
    public GameObject otherArmy;

    [Header("UI Setup")]
    public Text factionName;
    public Image homeFlag, awayFlag;
    public Text homeSoldiers, awaySoldiers;
    [Space]
    public Text dialogueTXT;


    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if(otherArmy != null)
        {
            dialogueTXT.text = dialogue;

            homeFlag.sprite = GameManager.instance.playerFactionObject.GetComponent<Faction>().flag;
            homeSoldiers.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().army.GetComponent<Army>().numOfSoldiers.ToString();

            awayFlag.sprite = otherfaction.GetComponent<Faction>().flag;
            awaySoldiers.text = otherfaction.GetComponent<Faction>().army.GetComponent<Army>().numOfSoldiers.ToString();
        }
    }

    public void AutoResolve()
    {
        dialogue += "Prepare to die. ";
        GameManager.instance.timeSpeed = 1f;
        Time.timeScale = 1;

        GameManager.instance.playerFactionObject.GetComponent<Faction>().army.GetComponent<Army>().Battle(otherArmy);
        //close UI after

    }

    public void Leave()
    {
        dialogue += "Fairwell then... ";
        otherfaction.GetComponent<AI_Faction>().GenerateNextTask();
        Invoke("CloseUI", 1f);
    }
    public void CloseUI()
    {
        dialogue = ""; // clear dialogue

        //reset buttons 
        foreach(Button button in GetComponentsInChildren<Button>())
        {
            button.interactable = true;
        }
        gameObject.SetActive(false);
    }
}
