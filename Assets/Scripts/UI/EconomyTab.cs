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

    [Header("Expenses")]
    public Text armyExpTXT;
    public Text totalExpensesTXT;

    [Header("Income")]
    public Text taxIncomeTXT;
    public Text tradeIncomeTXT;
    public Text totalIncomeTXT;

    [Header("Graph")]
    public UILineRenderer line;
    public UIGridRenderer grid;

    [Header("Budgets")]
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
        monthlyChangeTXT.text = (GameManager.instance.playerFactionObject.GetComponent<Faction>().taxIncome - GameManager.instance.playerFactionObject.GetComponent<Faction>().expenses).ToString("0.0");
        //income
        taxIncomeTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().taxIncome.ToString("0.0");
        tradeIncomeTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().monthlyTrade.ToString("0.0");
        totalIncomeTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().taxIncome.ToString("0.0");
        //expense
        armyExpTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().expenses.ToString("0.0");
        totalExpensesTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().expenses.ToString("0.0");

        //budgets
        GameManager.instance.playerFactionObject.GetComponent<Faction>().taxRate = taxSlider.value;

    }

    public void UpdateGraph(float currentTreasury)
    {
        grid.SetAllDirty();
        line.SetAllDirty();

        if (line.points.ToArray().Length < grid.gridSize.x + 1)
        {
            Vector2 newLine = new Vector2(line.points.ToArray().Length, currentTreasury);
            line.points.Add(newLine);
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
            Vector2 newLine = new Vector2(line.points.ToArray().Length, currentTreasury);
            line.points.Add(newLine);
            grid.gridSize.y = Mathf.RoundToInt(largest);
        }
        
        //colouring
        for (int i = 0; i < line.points.ToArray().Length; i++)
        {
            if(line.points.IndexOf(line.points[i]) == line.points.ToArray().Length)
            {
                if(line.points[i - 1].y > line.points[i].y)
                {
                    line.color = Color.red;
                }
                else
                {
                    line.color = Color.green;
                }
            }
        }
        if (currentTreasury > grid.gridSize.y)
        {
            grid.gridSize.y = Mathf.RoundToInt(currentTreasury);
        }
        grid.SetAllDirty();
        line.SetAllDirty();
    }
}
