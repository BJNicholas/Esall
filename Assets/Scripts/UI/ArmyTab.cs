using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmyTab : MonoBehaviour
{
    public Text totalHealth, numberOfUnits, foodReserves, milScore, monthlyCost;


    private void Update()
    {
        if(GameManager.instance.playerFactionObject.GetComponent<Faction>().army != null)
        {
            totalHealth.text = "%" + TotalHealthCalculation(GameManager.instance.playerFactionObject.GetComponent<Faction>().army.GetComponent<Army>()).ToString("F2");
            numberOfUnits.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().army.GetComponent<Army>().numOfSoldiers.ToString();
            foodReserves.text = FoodReservesCalculation(GameManager.instance.playerFactionObject.GetComponent<Faction>().army.GetComponent<Army>()).ToString();
            monthlyCost.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().armyWages.ToString();
        }
        else
        {
            totalHealth.text = "%0";
            numberOfUnits.text = "0";
            foodReserves.text = "0";
            monthlyCost.text = "0";  
            
        }

        milScore.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().ruler.GetComponent<Character>().mil.ToString();
        
    }


    public float TotalHealthCalculation(Army army)
    {
        float maxHealth = 0f;
        float currentHealth = 0f;
        for (int i = 0; i < army.soldiers.ToArray().Length; i++)
        {
            maxHealth += army.soldiers[i].maxHealth;
            currentHealth += army.soldiers[i].health;
        }
            
        float healthPercent = (currentHealth / maxHealth) * 100;
        return healthPercent;
    }

    public float FoodReservesCalculation(Army army)
    {
        float foodLeft = 0f;
        for (int i = 0; i < army.storedItems.ToArray().Length; i++)
        {
            if (army.storedItems[i].type == Item.ItemType.food) foodLeft += 1;
        }
        return foodLeft;
    }
}
