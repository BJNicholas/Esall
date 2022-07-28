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
    public Text incomeTXT;
    public Text expensesTXT;


    [Header("Time/Date Info")]
    public Text hour;
    public Text day, month, year;

    private void Start()
    {
        flag.sprite = GameManager.instance.playerFactionObject.GetComponent<Faction>().flag;
        factionNameTXT.text = GameManager.instance.playerFaction.ToString();
    }

    private void Update()
    {
        treasuryTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury.ToString("0.0");
        incomeTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().taxIncome.ToString("0.0");
        expensesTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().expenses.ToString("0.0");
        hour.text = Mathf.Round(GameManager.instance.hour).ToString();
        day.text = GameManager.instance.date.x.ToString();
        month.text = GameManager.instance.date.y.ToString();
        year.text = GameManager.instance.date.z.ToString();
    }


    public void ChangeTimeSpeed(float newSpeed)
    {
        GameManager.instance.timeSpeed = newSpeed;
    }
}
