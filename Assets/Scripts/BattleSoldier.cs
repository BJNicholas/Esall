using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSoldier : MonoBehaviour
{
    public Soldier scriptable;

    [Header("UI Setup")]
    public Gradient heathColours;
    public Text healthTXT;

    private void Start()
    {
        healthTXT.transform.parent.transform.localScale = new Vector3(transform.localScale.x, 1, 1);
    }


    private void Update()
    {
        healthTXT.color = heathColours.Evaluate(scriptable.heath / 100);
        healthTXT.text = scriptable.heath.ToString();
    }


    public void March()
    {
        GetComponent<Animator>().Play("March");
    }
    public void Attack()
    {
        GetComponent<Animator>().Play("Attack");
    }
    public void Block()
    {
        GetComponent<Animator>().Play("Block");
    }
}
