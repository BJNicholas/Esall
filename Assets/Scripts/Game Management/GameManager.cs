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

    [Header("Prefabs")]
    public GameObject merchantPrefab;
    public GameObject armyPrefab;
    public Soldier[] soldierTypes;
    public GameObject eventPrefab;
    public GameObject pauseMenu;

    [Header("Game Details")]
    public FactionManager.factions playerFaction;
    public GameObject playerFactionObject;
    [Header("Time")]
    public float timeSpeed = 1;
    public float hour;
    public Vector3 date;

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
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void CalculateTime()
    {
        Time.timeScale = timeSpeed;
        hour += 1 * timeSpeed * Time.deltaTime;
        if(hour > 24) //end of Day
        {
            hour = 0;
            date.x += 1;
        }
        if(date.x >= 32) //end of Month
        {
            date.x = 1;
            date.y += 1;
            //monthly Taxes from provinces
            foreach (GameObject faction in FactionManager.instance.factionObjects)
            {
                faction.GetComponent<Faction>().chargeTax();
                faction.GetComponent<Faction>().PayExpenses();
                GetComponent<AudioSource>().Play();

            }

            foreach (GameObject resource in resources) resource.GetComponent<ProductionManager>().Produce();

        }
        if (date.y >= 13) //end of Year
        {
            date.y = 1;
            date.z += 1;
        }
    }

    public void Save()
    {
        //resetting arrays/lists
        armies.Clear();
        merchants.Clear();

        //factions
        foreach (GameObject faction in FactionManager.instance.factionObjects)
        {
            SaveSystem.SaveFaction(faction.GetComponent<Faction>());
            armies.Add(faction.GetComponent<Faction>().army); // populate list
        }
        //armies
        foreach(GameObject army in armies)
        {
            SaveSystem.SaveArmy(army.GetComponent<Army>());
        }
        //merchants
        foreach(GameObject settlement in settlements)
        {
            foreach(GameObject merchant in settlement.GetComponent<Settlement>().merchants)
            {
                merchants.Add(merchant);
                SaveSystem.SaveMerchant(merchant.GetComponent<Merchant>());
            }
        }
    }

    public void Load()
    {
        //factions
        foreach (GameObject faction in FactionManager.instance.factionObjects)
        {
            FactionData data = SaveSystem.loadFaction(faction.GetComponent<Faction>());

            //easy conversion

            faction.GetComponent<Faction>().treasury = data.treasury;
            faction.GetComponent<Faction>().atWar = data.atWar;

            //complex conversion

            //clearing arrays to then be filled
            faction.GetComponent<Faction>().ownedTiles.Clear();
            faction.GetComponent<Faction>().allies.Clear();
            faction.GetComponent<Faction>().enemies.Clear();

            //owned
            for (int i = 0; i < data.ownedTiles.Length; i++)
            {
                foreach (GameObject tile in GameManager.instance.provinces)
                {
                    if(data.ownedTiles[i] == tile.GetComponent<Tile>().id)
                    {
                        faction.GetComponent<Faction>().ownedTiles.Add(tile);
                    }
                }
            }
            if(faction.GetComponent<Faction>().ownedTiles.ToArray().Length > 0)
            {
                faction.SetActive(true);
                faction.GetComponent<Faction>().UpdateOwnedTiles();
            }
            //allies
            for (int i = 0; i < data.allies.Length; i++)
            {
                foreach (GameObject nation in FactionManager.instance.factionObjects)
                {
                    if (data.allies[i] == nation.GetComponent<Faction>().id)
                    {
                        faction.GetComponent<Faction>().allies.Add(nation);
                    }
                }
            }
            //enemies
            for (int i = 0; i < data.enemies.Length; i++)
            {
                foreach (GameObject nation in FactionManager.instance.factionObjects)
                {
                    if (data.enemies[i] == nation.GetComponent<Faction>().id)
                    {
                        faction.GetComponent<Faction>().enemies.Add(nation);
                    }
                }
            }
            if(faction != playerFactionObject)
            {
                faction.GetComponent<AI_Faction>().GenerateNextTask();
            }
        }
        //armies
        foreach(GameObject army in armies)
        {
            ArmyData data = SaveSystem.loadArmy(army.GetComponent<Army>());

            //easy conversion

            army.GetComponent<Army>().isPlayer = data.isPlayer;
            army.SetActive(data.alive);

            //complex conversion

            //clearing arrays to then be filled
            army.GetComponent<Army>().soldiers.Clear();
            army.GetComponent<Army>().storedItems.Clear();

            //target
            army.GetComponent<Army>().target = new Vector3(data.target[0], data.target[1], data.target[2]);

            //position
            army.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            //soldiers
            for (int i = 0; i < data.soldiers.Length; i++)
            {
                foreach(Soldier soldier in GameManager.instance.soldierTypes)
                {
                    if (data.soldiers[i] == soldier.id)
                    {
                        army.GetComponent<Army>().soldiers.Add(soldier);
                    }
                }
            }
            //items
            for (int i = 0; i < data.storedItems.Length; i++)
            {
                foreach (Item item in ResourceManager.instance.items)
                {
                    if (data.storedItems[i] == item.id)
                    {
                        army.GetComponent<Army>().storedItems.Add(item);
                    }
                }
            }
        }
        //merchants
        foreach(GameObject merchant in merchants)
        {
            MerchantData data = SaveSystem.loadMerchant(merchant.GetComponent<Merchant>());

            //easy conversion

            merchant.GetComponent<Merchant>().treasury = data.treasury;
            merchant.GetComponent<Merchant>().oldTreasuryValue = data.oldValue;

            //complex conversion

            //clearing arrays to then be filled
            merchant.GetComponent<Merchant>().storedItems.Clear();

            //target
            merchant.GetComponent<Merchant>().target = new Vector3(data.target[0], data.target[1], data.target[2]);

            //position
            merchant.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            //items
            for (int i = 0; i < data.storedItems.Length; i++)
            {
                foreach (Item item in ResourceManager.instance.items)
                {
                    if (data.storedItems[i] == item.id)
                    {
                        merchant.GetComponent<Merchant>().storedItems.Add(item);
                    }
                }
            }
            //home
            foreach(GameObject tile in GameManager.instance.provinces)
            {
                if(tile.GetComponent<Tile>().id == data.homeProvID)
                {
                    merchant.GetComponent<Merchant>().homeCity = tile.GetComponent<Tile>().settlement;
                }
            }
        }
    }


}
