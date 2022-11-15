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
    public GameObject leaveButton;
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
            if (GameManager.instance.playerFactionObject.GetComponent<Faction>().army != null)
            {
                homeSoldiers.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().army.GetComponent<Army>().numOfSoldiers.ToString();
            }
            else
            {
                homeSoldiers.text = "0";
            }

            awayFlag.sprite = otherfaction.GetComponent<Faction>().flag;
            if (otherfaction.GetComponent<Faction>().army != null)
            {
                awaySoldiers.text = otherfaction.GetComponent<Faction>().army.GetComponent<Army>().numOfSoldiers.ToString();
            }
            else
            {
                awaySoldiers.text = "0";
            }


            if (GameManager.instance.playerFactionObject.GetComponent<Faction>().enemies.Contains(otherfaction))
            {
                leaveButton.SetActive(false);
            }
            else
            {
                leaveButton.SetActive(true);
            }
        }
        else
        {
            dialogue += "My land has fallen to foreign invaders, remember me...";
            Invoke("CloseUI", 1f);
        }
    }

    public void AutoResolve()
    {
        dialogue += "Prepare to die. ";
        GameManager.instance.timeSpeed = 1f;
        Time.timeScale = 1;
        DiploHub.instance.gameObject.SetActive(false);

        if (!GameManager.instance.playerFactionObject.GetComponent<Faction>().enemies.Contains(otherfaction))
        {
            DiploHub.instance.faction = otherfaction;
            DiploHub.instance.DeclareWar();
        }

        
        Battle.instance.BattleStart(GameManager.instance.playerFactionObject.GetComponent<Faction>().army, otherArmy);

        dialogue = ""; // clear dialogue
        //reset buttons 
        foreach (Button button in GetComponentsInChildren<Button>())
        {
            button.interactable = true;
        }
        gameObject.SetActive(false);

    }

    public void Diplomacy()
    {
        DiploHub.instance.gameObject.SetActive(true);
        DiploHub.instance.faction = otherfaction;
        DiploHub.instance.SetDecisions();
        DiploHub.instance.SetDecisions();
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
