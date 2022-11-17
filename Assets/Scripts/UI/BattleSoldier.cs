using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class BattleSoldier : MonoBehaviour
{
    public Soldier soldier;
    public GameObject enemyArmy;

    [Space]
    public bool fighting, dead;

    [Header("UI Setup")]
    public Image icon;
    public Text displayName;
    public Slider health, charge;

    private void Start()
    {
        icon.sprite = soldier.icon;
        displayName.text = soldier.name;
        health.maxValue = soldier.maxHealth;
    }


    private void Update()
    {
        health.value = soldier.health;
        charge.value = soldier.charge;

        if(soldier.charge >= 100)
        {
            print(enemyArmy.name + " " + soldier);
            Attack();
            soldier.charge = 0;
        }

        if(soldier.health <= 0)
        {
            Death();
        }
    }

    public void Attack()
    {
        Soldier enemySoldier = null;
        if (enemyArmy == Battle.instance.player)
        {
            enemySoldier = Battle.instance.playerFL.gameObject.GetComponentInChildren<BattleSoldier>().soldier;
        }
        else
        {
            enemySoldier = Battle.instance.aiFL.gameObject.GetComponentInChildren<BattleSoldier>().soldier;
        }
        enemySoldier.health -= soldier.damage;
        Battle.instance.FrontLineCheck();
    }

    void Death()
    {
        dead = true;
        fighting = false;
        soldier.charge = 0;

        Battle.instance.FrontLineCheck();
    }

    private void FixedUpdate()
    {
        if (fighting && Battle.instance.fighting)
        {
            soldier.charge += (soldier.chargeSpeed * 3)* GameManager.instance.timeSpeed * Time.deltaTime;
        }
        else
        {
            soldier.charge = 0;
        }
    }
}
