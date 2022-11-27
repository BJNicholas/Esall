using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<GameObject> provinces;
    public List<GameObject> armies;
    public List<GameObject> merchants;
    public List<GameObject> settlements;
    public List<GameObject> resources;
    public List<GameObject> manpowerResources;
    public Modifier[] modifiers;

    [Header("Prefabs")]
    public GameObject merchantPrefab;
    public GameObject armyPrefab;
    public Soldier[] soldierTypes;
    public GameObject eventPrefab;

    [Header("Game Details")]
    public FactionManager.factions playerFaction;
    public GameObject playerFactionObject;
    [Header("Time")]
    public float timeSpeed = 1;
    public float hour;
    public Vector3 date;

    [Header("Setup")]
    public GameObject pauseMenu;
    //win and lose screens
    public GameObject win, lose;

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1;
        foreach(GameObject tile in provinces)
        {
            tile.GetComponent<Tile>().id = provinces.IndexOf(tile);
        }
    }

    private void FixedUpdate()
    {
        CalculateTime();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause.instance.PauseGame();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            StartCoroutine(ChangeSpeedShortcut());
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if(Console.instance.gameObject.activeInHierarchy) Console.instance.gameObject.SetActive(false);
            else Console.instance.gameObject.SetActive(true);
        }
    }

    public IEnumerator ChangeSpeedShortcut()
    {
        yield return new WaitForEndOfFrame();
        if (timeSpeed == 1)
        {
            timeSpeed = 2.5f;
            StopCoroutine(ChangeSpeedShortcut());
        }
        else if (timeSpeed == 2.5f)
        {
            timeSpeed = 5f;
            StopCoroutine(ChangeSpeedShortcut());
        }
        else if (timeSpeed == 5f)
        {
            timeSpeed = 1f;
            StopCoroutine(ChangeSpeedShortcut());
        }

    }
    public void CalculateTime()
    {
        //Time.timeScale = timeSpeed;
        hour += (3.5f)* timeSpeed * Time.deltaTime;
        if(hour > 24) //end of Day
        {
            hour = 0;
            date.x += 1;
            if(playerFactionObject != null || playerFaction != FactionManager.factions.Observer)
            {
                EconomyTab.instance.UpdateGraph(playerFactionObject.GetComponent<Faction>().treasury);
                GovernmentTab.instance.UpdateGraph(playerFactionObject.GetComponent<Faction>().publicOrder);
            }
        }
        if(date.x >= 32) //end of Month
        {
            date.x = 1;
            date.y += 1;

            //monthly public order changes in each province
            foreach(GameObject tile in provinces)
            {
                tile.GetComponent<Tile>().PublicOrderChange();
                tile.GetComponent<Tile>().ConversionCheck();
            }
            //monthly Taxes from provinces
            foreach (GameObject faction in FactionManager.instance.factionObjects)
            {
                GetComponent<AudioSource>().Play();

                faction.GetComponent<Faction>().monthlyTrade = 0; //reset monthly trade profit

                if (faction.activeInHierarchy)
                {

                    faction.GetComponent<Faction>().AdjustRelations(); //revise the current relations in regard to their modifiers
                    faction.GetComponent<Faction>().ChargeForIncome();
                    faction.GetComponent<Faction>().PayExpenses();

                    EventFirer.instance.FireEvent(faction); // add rng events for every faction
                    foreach (NationalModifier natMod in faction.GetComponent<Faction>().nationalMods)
                    {
                        natMod.TakeAction();
                        if (natMod.lifeSpan <= 0) StartCoroutine(natMod.RemoveMod(1.5f));
                    }
                }

            }

            foreach (GameObject resource in resources) resource.GetComponent<ProductionManager>().Produce();
            foreach (GameObject resource in manpowerResources) resource.GetComponent<SoldierProduction>().Produce();

            CharacterManager.instance.AjustLives();

            GovernmentTab.instance.UpdateNationalModsList();

        }
        if (date.y >= 13) //end of Year
        {
            date.y = 1;
            date.z += 1;
        }
    }



}
