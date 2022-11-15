using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battle : MonoBehaviour
{
    public static Battle instance;
    public bool fighting = false;

    public GameObject player, AI; //these are the army objects of both

    [Header("Prefabs")]
    public GameObject BattleSoldier; //the basic prefab

    [Header("generic UI Setup")]
    public GameObject afterBattlePopUp;
    public Text victorText;
    public Image playerFlag, aiFlag;

    [Header("Player UI SetUp")]
    public Transform playerFL;
    public Transform playerRes, playerSup, playerLos;

    [Header("AI UI SetUp")]
    public Transform aiFL;
    public Transform aiRes, aiSup, aiLos;

    private void Start()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void BattleStart(GameObject playerArmy, GameObject aiArmy)
    {
        GetComponent<AudioSource>().Play();
        gameObject.SetActive(true);
        afterBattlePopUp.SetActive(false);

        player = playerArmy;
        AI = aiArmy;
        PopulateArmy(player);
        PopulateArmy(AI);

        playerFlag.sprite = player.GetComponent<Army>().ownerObject.GetComponent<Faction>().flag;
        aiFlag.sprite = AI.GetComponent<Army>().ownerObject.GetComponent<Faction>().flag;

        fighting = true;
        FrontLineCheck();
    }

    public void FrontLineCheck()
    {
        List<Transform> frontLines = new List<Transform>();
        frontLines.Add(playerFL);
        frontLines.Add(aiFL);

        foreach(Transform fl in frontLines)
        {
            if(fl.childCount == 0)
            {
                if (fl == playerFL)
                {
                    print("assigning player FL");
                    AssignFrontLine(player, fl);
                }
                else
                {
                    print("assigning enemy FL");
                    AssignFrontLine(AI, fl);
                }
            }
            else
            {
                print("Front lines both full");
                if (fl.GetComponentInChildren<BattleSoldier>().dead)
                {
                    if (fl == playerFL) AssignSoldierToList(fl.GetChild(0).gameObject, playerLos);
                    else AssignSoldierToList(fl.GetChild(0).gameObject, aiLos);

                    FrontLineCheck();
                }
            }
        }
    }

    void AssignFrontLine(GameObject army, Transform frontLine)
    {
        GameObject flSoldier = null;

        if(army == player)
        {
            if (playerRes.childCount > 0)
            {
                flSoldier = playerRes.GetChild(0).gameObject;
                AssignSoldierToList(flSoldier, frontLine);
                flSoldier.GetComponent<BattleSoldier>().fighting = true;
            }
            else
            {
                if(playerSup.childCount > 0)
                {
                    flSoldier = playerSup.GetChild(0).gameObject;
                    AssignSoldierToList(flSoldier, frontLine);
                }
                else
                {
                    GetComponent<AudioSource>().Stop();
                    fighting = false;
                    afterBattlePopUp.SetActive(true);
                    victorText.text = AI.GetComponent<Army>().owner.ToString() + " Wins!";
                    AI.GetComponent<Army>().ownerObject.GetComponent<AI_Faction>().GenerateNextTask();
                }
            }
        }
        else
        {
            if (aiRes.childCount > 0)
            {
                flSoldier = aiRes.GetChild(0).gameObject;
                AssignSoldierToList(flSoldier, frontLine);
                flSoldier.GetComponent<BattleSoldier>().fighting = true;
            }
            else
            {
                if (aiSup.childCount > 0)
                {
                    flSoldier = aiSup.GetChild(0).gameObject;
                    AssignSoldierToList(flSoldier, frontLine);
                }
                else
                {
                    GetComponent<AudioSource>().Stop();
                    fighting = false;
                    afterBattlePopUp.SetActive(true);
                    victorText.text = player.GetComponent<Army>().owner.ToString() + " Wins!";
                }
            }
        }
    }

    void PopulateArmy(GameObject army)
    {
        foreach(Soldier soldier in army.GetComponent<Army>().soldiers)
        {
            GameObject newBattleSoldier = Instantiate(BattleSoldier);
            //make a copy, dont draw from the original scriptable Object
            newBattleSoldier.GetComponent<BattleSoldier>().soldier = soldier;

            if(army == player)
            {
                if (soldier.supportUnit)
                {
                    AssignSoldierToList(newBattleSoldier, playerSup);
                    newBattleSoldier.GetComponent<BattleSoldier>().fighting = true;
                }
                else AssignSoldierToList(newBattleSoldier, playerRes);

                newBattleSoldier.GetComponent<BattleSoldier>().enemyArmy = AI;

            }
            else
            {
                if (soldier.supportUnit)
                {
                    AssignSoldierToList(newBattleSoldier, aiSup);
                    newBattleSoldier.GetComponent<BattleSoldier>().fighting = true;
                }
                else AssignSoldierToList(newBattleSoldier, aiRes);

                newBattleSoldier.GetComponent<BattleSoldier>().enemyArmy = player;
            }
        }
    }

    public void AssignSoldierToList(GameObject soldier,Transform list)
    {
        soldier.transform.parent = list;
    }


    public void CloseBattle()
    {
        for (int i = 0; i < playerLos.childCount; i++)
        {
            player.GetComponent<Army>().soldiers.Remove(playerLos.GetChild(i).gameObject.GetComponent<BattleSoldier>().soldier);
        }
        for (int i = 0; i < aiLos.childCount; i++)
        {
            AI.GetComponent<Army>().soldiers.Remove(aiLos.GetChild(i).gameObject.GetComponent<BattleSoldier>().soldier);
        }


        List<Transform> allLists = new List<Transform>();
        allLists.Add(playerFL);
        allLists.Add(playerRes);
        allLists.Add(playerSup);
        allLists.Add(playerLos);
        allLists.Add(aiFL);
        allLists.Add(aiRes);
        allLists.Add(aiSup);
        allLists.Add(aiLos);

        for (int i = 0; i < allLists.ToArray().Length; i++)
        {
            for (int y = 0; y < allLists[i].childCount; y++)
            {
                Destroy(allLists[i].GetChild(y).gameObject);
            }
        }

        victorText.text = "";
        gameObject.SetActive(false);

    }
}
