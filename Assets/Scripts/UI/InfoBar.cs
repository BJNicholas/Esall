using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoBar : MonoBehaviour
{
    [Header("Faction Info")]
    public Image flag;
    public Text factionNameTXT;
    [Header("Economy")]
    public Text treasuryTXT;
    public Text monthlyChangeTXT;


    [Header("Time/Date Info")]
    public Text hour;
    public Text day, month, year;

    private void Awake()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    private void Start()
    {
        flag.sprite = GameManager.instance.playerFactionObject.GetComponent<Faction>().flag;
        factionNameTXT.text = GameManager.instance.playerFaction.ToString();
    }

    private void Update()
    {
        //updating the faction macro tabs
        if(GameManager.instance.playerFaction != FactionManager.factions.Observer)
        {
            GovernmentTab.instance.UpdateINFO();
            EconomyTab.instance.UpdateINFO();
        }


        treasuryTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury.ToString("0.0");
        if(GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury > 1000)
        {
            treasuryTXT.text = (GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury / 1000).ToString("F") + "K";
        }
        if (GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury > 1000000)
        {
            treasuryTXT.text = (GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury / 1000000).ToString("F") + "M";
        }
        //monthlyChangeTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().income.ToString("0.0");
        hour.text = Mathf.Round(GameManager.instance.hour).ToString();
        day.text = GameManager.instance.date.x.ToString();
        month.text = GameManager.instance.date.y.ToString();
        year.text = GameManager.instance.date.z.ToString();
    }


    public void ChangeTimeSpeed(float newSpeed)
    {
        GameManager.instance.timeSpeed = newSpeed;
    }

    public void ToggleTab(GameObject tab)
    {
        if (tab.activeInHierarchy) tab.SetActive(false);
        else tab.SetActive(true);
        tab.GetComponent<AudioSource>().Play();
    }
}
