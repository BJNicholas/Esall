using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Army : MonoBehaviour
{
    public FactionManager.factions owner;
    [HideInInspector]public GameObject ownerObject;
    public GameObject currentProvince;
    [Header("Setup")]
    public bool isPlayer;
    public Text stateTXT;
    public Text factionTXT;
    bool inBattle = false;
    public bool isGarrison = false;

    [Header("Stats")]
    public int numOfSoldiers = 1;
    public float strength; // total combined health of all soldiers as a percentage of what the max would be
    public List<Soldier> soldiers;

    [Header("NavMesh Details")]
    public Vector3 target;
    private NavMeshAgent agent;
    public float speed = 0.25f;

    [Header("Trade")]
    public List<Item> storedItems;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        target = gameObject.transform.position;

        factionTXT.text = owner.ToString();
        if (isPlayer)
        {
            gameObject.AddComponent<PlayerController>();
            stateTXT.gameObject.SetActive(false);
            Inventory.instance.army = gameObject;
        }
        else
        {
            gameObject.AddComponent<AI_ArmyController>();
        }
        float armyCost = 0;
        ownerObject.GetComponent<Faction>().expenses += GenerateEntireArmyCPM(armyCost);

        transform.rotation = Quaternion.identity; // fixes rotation bug that makes armies/merhcants invisible

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Tile")
        {
            currentProvince = collision.gameObject;
        }
    }

    private void Update()
    {
        agent.speed = speed * GameManager.instance.timeSpeed;
        agent.SetDestination(target);
        numOfSoldiers = soldiers.ToArray().Length;
        if (Input.GetKeyDown(KeyCode.I) && Inventory.instance.gameObject.activeInHierarchy == false)
        {
            Inventory.instance.gameObject.SetActive(true);
            Inventory.instance.GenerateListings();
            Inventory.instance.LiveUpdateListings();
        }

        if (inBattle) target = transform.position;

        if (numOfSoldiers == 0) Death();
    }

    public void Death()
    {
        gameObject.SetActive(false);

        transform.position = ownerObject.GetComponent<Faction>().capitalCity.transform.position;
        Destroy(gameObject);
        ownerObject.GetComponent<Faction>().Invoke("SpawnNewArmy", 30f);
        if (owner != GameManager.instance.playerFaction)
        {
            ownerObject.GetComponent<AI_Faction>().currentTask = AI_Faction.PossibleTasks.Null;
            ownerObject.GetComponent<AI_Faction>().StopAllCoroutines();
        }
        else
        {
            if (SettlementInspector.instance.gameObject.activeInHierarchy)
            {
                SettlementInspector.instance.CloseAllDown();
            }
        }
    }

    float GenerateEntireArmyCPM(float cpm)
    {
        foreach(Soldier unit in soldiers)
        {
            cpm += unit.cpm;
        }

        return cpm;
    }

    public void Battle(GameObject defendingArmy)
    {
        inBattle = true;
        if(numOfSoldiers > 0 && defendingArmy.GetComponent<Army>().numOfSoldiers > 0)
        {
            StartCoroutine(BattleTick(defendingArmy));
        }
        else
        {
            inBattle = false;
            //BATTLE OVER
            if (isGarrison)
            {
                currentProvince.GetComponent<Tile>().settlement.GetComponent<Settlement>().garrisonSize = soldiers.ToArray().Length;
            }
            print("BATTLE OVER");
            if (numOfSoldiers == 0)
            {
                print("DEFENDER WINS");
                Encounter.instance.dialogue += " Better luck next time child.";
                Encounter.instance.Invoke("CloseUI", 1.5f);
            }
            else
            {
                print("ATTACKER WINS");
                Encounter.instance.dialogue += " You have bested me.";
                Encounter.instance.Invoke("CloseUI", 1.5f);
            }

            //resetting AI after battle
            if (ownerObject != GameManager.instance.playerFactionObject && gameObject.activeInHierarchy)
            {
                ownerObject.GetComponent<AI_Faction>().taskComplete = true;
                if (defendingArmy.GetComponent<Army>().ownerObject != GameManager.instance.playerFactionObject && defendingArmy.activeInHierarchy)
                {
                    defendingArmy.GetComponent<Army>().ownerObject.GetComponent<AI_Faction>().taskComplete = true;
                }

            }
            else if (defendingArmy.GetComponent<Army>().ownerObject != GameManager.instance.playerFactionObject && defendingArmy.activeInHierarchy)
            {
                defendingArmy.GetComponent<Army>().ownerObject.GetComponent<AI_Faction>().taskComplete = true;
                if (ownerObject != GameManager.instance.playerFactionObject && gameObject.activeInHierarchy)
                {
                    ownerObject.GetComponent<AI_Faction>().taskComplete = true;
                }
            }
        }
    }

    public IEnumerator BattleTick(GameObject defendingArmy)
    {
        yield return new WaitForSeconds(1.5f);

        Soldier attacker, defender; //assigning vars
        attacker = soldiers[0];
        defender = defendingArmy.GetComponent<Army>().soldiers[0];

        //debug for now --- only a 50/50 chance -- will change
        int roll = Random.Range(0, 2);
        if(roll == 0) //attacker wins
        {
            print(ownerObject.name + " WINS BATTLE TICK");
            defendingArmy.GetComponent<Army>().ownerObject.GetComponent<Faction>().expenses -= defender.cpm;
            defendingArmy.GetComponent<Army>().soldiers.Remove(defender);
            defendingArmy.GetComponent<Army>().numOfSoldiers -= 1;
        }
        else //defender wins
        {
            print(defendingArmy.GetComponent<Army>().ownerObject.name + " WINS BATTLE TICK");
            ownerObject.GetComponent<Faction>().expenses -= attacker.cpm;
            soldiers.Remove(attacker);
            numOfSoldiers -= 1;
        }


        Battle(defendingArmy);
    }



}
