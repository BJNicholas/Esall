using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EconomyTab : MonoBehaviour
{
    public static EconomyTab instance;
    [Header("MACRO")]
    public Text treasuryTXT;
    public Text monthlyChangeTXT;
    public Text macroChangeTXT;

    [Header("Expenses")]
    public Text armyExpTXT;
    public Text merchantExpTXT;
    public Text totalExpensesTXT;

    [Header("Income")]
    public Text taxIncomeTXT;
    public Text EffectfromPO;
    public Text tradeIncomeTXT;
    public Text merchantCountTXT;

    [Header("Graph")]
    public UILineRenderer line;
    public UIGridRenderer grid;

    [Header("Budgets")]
    public Slider armyWageSlider;
    public Slider merchantWageSlider;
    public Slider taxSlider;


    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void UpdateINFO()
    {
        //totals
        treasuryTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().treasury.ToString("0.0");
        float monthlyChange = (GameManager.instance.playerFactionObject.GetComponent<Faction>().income - GameManager.instance.playerFactionObject.GetComponent<Faction>().expenses);
        monthlyChangeTXT.text = monthlyChange.ToString("0.0");
        macroChangeTXT.text = (GameManager.instance.playerFactionObject.GetComponent<Faction>().income - GameManager.instance.playerFactionObject.GetComponent<Faction>().expenses).ToString("0.0");
        //colouring
        if (float.Parse(monthlyChangeTXT.text) > 0)
        {
            monthlyChangeTXT.color = Color.green;
            macroChangeTXT.color = Color.green;
        }
        else
        {
            monthlyChangeTXT.color = Color.red; 
            macroChangeTXT.color = Color.red;
        }
        //simplfying
        if (monthlyChange > 1000)
        {
            macroChangeTXT.text = (monthlyChange / 1000).ToString("F") + "K";
        }
        if (monthlyChange > 1000000)
        {
            macroChangeTXT.text = (monthlyChange / 1000000).ToString("F") + "M";
        }

        //income
        taxIncomeTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().taxIncome.ToString("0.0");
        EffectfromPO.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().PO_bonus.ToString("0.0");
        //colouring
        if (float.Parse(EffectfromPO.text) > 0)
        {
            EffectfromPO.color = Color.green;
        }
        else
        {
            EffectfromPO.color = Color.red;
        }
        tradeIncomeTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().monthlyTrade.ToString("0.0");
        merchantCountTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().numOfMerchants.ToString("0");
        //expense
        armyExpTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().armyWages.ToString("0.0");
        merchantExpTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().merchantWages.ToString("0.0");
        totalExpensesTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().expenses.ToString("0.0");

        //budgets
        GameManager.instance.playerFactionObject.GetComponent<Faction>().taxRate = taxSlider.value;
        GameManager.instance.playerFactionObject.GetComponent<Faction>().armyWageRate = armyWageSlider.value;
        GameManager.instance.playerFactionObject.GetComponent<Faction>().merchantWageRate = merchantWageSlider.value;

    }

    public void UpdateGraph(float currentTreasury)
    {
        grid.SetAllDirty();
        line.SetAllDirty();

        Vector2 newLine;

        if (line.points.ToArray().Length < grid.gridSize.x + 1)
        {
            newLine = new Vector2(line.points.ToArray().Length, currentTreasury);
        }
        else
        {
            line.points.Remove(line.points[0]);
            float largest = 0;
            for (int i = 0; i < line.points.ToArray().Length; i++)
            {
                line.points[i] = new Vector2(line.points[i].x - 1, line.points[i].y);
                if (line.points[i].y > largest) largest = line.points[i].y;
            }
            newLine = new Vector2(line.points.ToArray().Length, currentTreasury);
            grid.gridSize.y = Mathf.RoundToInt(largest);
        }

        if(currentTreasury <= 0) // to make sure it doesn't clip off the edge
        {
            newLine = new Vector2(line.points.ToArray().Length, 0);
        }
        line.points.Add(newLine);
        float monthlyChange = (GameManager.instance.playerFactionObject.GetComponent<Faction>().income - GameManager.instance.playerFactionObject.GetComponent<Faction>().expenses);
        //colouring
        if (monthlyChange > 0)
        {
            line.color = Color.green;
        }
        else line.color = Color.red;


        if (currentTreasury > grid.gridSize.y)
        {
            grid.gridSize.y = Mathf.RoundToInt(currentTreasury);
        }
        grid.SetAllDirty();
        line.SetAllDirty();
    }
}
