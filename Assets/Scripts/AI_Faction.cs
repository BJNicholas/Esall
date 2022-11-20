using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class AI_Faction : MonoBehaviour
{
    public enum leaderType
    {
        Administrator,
        Warmonger,
        Diplomat
    }
    public enum PossibleTasks
    {
        Null,
        //speaking to player
        Encounter,

        //civil
        Develop,
        Stockpile,
        Trade,
        //both
        Invest,
        Patrol,
        Muster,
        //war
        Attack,
        Siege, 
        //both
        Strategise,
        //Diplomacy
        Negotiate
    }

    public leaderType personality;
    public PossibleTasks currentTask;
    public float processingSpeed = 30;[Tooltip("time in seconds it takes for the AI to actually perform the task")]
    public GameObject army;

    public GameObject oldSettlement;
    public GameObject chosenSettlement;
    public bool arrived = false;
    public bool taskComplete = true;

    private void Start()
    {
        processingSpeed = Random.Range(8f, 12f);
        army = GetComponent<Faction>().army;
    }

    public void CreatePersonality()
    {
        int roll = Random.Range(0, 3);
        if (roll == 0) personality = leaderType.Administrator;
        else if (roll == 1) personality = leaderType.Warmonger;
        else if (roll == 2) personality = leaderType.Diplomat;
    }

    private void Update()
    {
        if(currentTask == PossibleTasks.Encounter)
        {
            StopAllCoroutines();
            CancelCurrentTask();
        }
        //print(gameObject.name + oldSettlement + " object");
        if (oldSettlement == chosenSettlement && arrived == false)
        {
            arrived = true;
            print(gameObject.name + " HAS CHOSEN THEIR CURRENT SETTLEMENT");
        }
        if (GetComponent<Faction>().atWar)
        {
            if(GetComponent<Faction>().enemies.ToArray().Length == 0)
            {
                GetComponent<Faction>().atWar = false;
                CancelCurrentTask();
            }
        }

        if (GetComponent<Faction>().army != null)
        {
            army = GetComponent<Faction>().army;
            if (chosenSettlement != null) army.GetComponent<Army>().target = chosenSettlement.transform.position;
            else
            {
                print(gameObject.name + " Looking for location to travel to");
                //this is a debug but it works
                //chosenSettlement = army;
                taskComplete = true;
                CancelCurrentTask();
            }

        }
        else
        {
            StopAllCoroutines();
            chosenSettlement = gameObject.GetComponent<Faction>().capitalCity;
        }

        if (taskComplete)
        {
            //running the task
            GenerateNextTask();
            RunTask(currentTask.ToString(), processingSpeed);
            taskComplete = false;
        }

        if(gameObject.GetComponent<Faction>().ruler == null)
        {
            gameObject.GetComponent<Faction>().ruler = CharacterManager.instance.CreateNewCharacter(gameObject.GetComponent<Faction>().ruler, gameObject.GetComponent<Faction>().capitalCity);
            CreatePersonality();
            print(gameObject.name + " created a new ruler, with the " + personality.ToString() + " personality");
        }
    }

    public void CancelCurrentTask()
    {
        if(currentTask != PossibleTasks.Encounter)
        {
            print(gameObject.name + " CANCELLED " + currentTask);
            chosenSettlement = army;
            StopAllCoroutines();
            taskComplete = true;
            GenerateNextTask();
        }
        else
        {
            print(gameObject.name + " CANCELLED TASK FOR ENCOUNTER");
            //remeber to set taskComplete where ever this finishes
            chosenSettlement = army;
        }
        
    }


    public void GenerateNextTask()
    {
        taskComplete = true;
        if (GetComponent<Faction>().atWar)
        {
            int roll = Random.Range(0, 4); // more likely to attack provinces - add attack soon
            if (roll == 0) currentTask = PossibleTasks.Muster;
            if (roll == 1) currentTask = PossibleTasks.Attack;
            //if (roll == 2) currentTask = PossibleTasks.Negotiate;
            else currentTask = PossibleTasks.Siege;
        }
        else
        {
            if (personality == leaderType.Administrator)
            {
                int roll = Random.Range(0, 6); // possible choices, increase number if more are added
                if (roll == 0) currentTask = PossibleTasks.Develop;
                else if (roll == 1) currentTask = PossibleTasks.Stockpile;
                else if (roll == 2) currentTask = PossibleTasks.Patrol;
                else if (roll == 3) currentTask = PossibleTasks.Muster;
                else if (roll == 4) currentTask = PossibleTasks.Invest;
                else if (roll == 5) currentTask = PossibleTasks.Trade;
            }
            else if (personality == leaderType.Warmonger)
            {
                int roll = Random.Range(0, 5); // possible choices, increase number if more are added
                if (roll == 0) currentTask = PossibleTasks.Muster;
                else if (roll == 1) currentTask = PossibleTasks.Strategise;
                else if (roll == 2) currentTask = PossibleTasks.Patrol;
                else if (roll == 3) currentTask = PossibleTasks.Develop;
                else if (roll == 4) currentTask = PossibleTasks.Invest;
            }
            else if (personality == leaderType.Diplomat)
            {
                int roll = Random.Range(0, 6); // possible choices, increase number if more are added
                if (roll == 0) currentTask = PossibleTasks.Develop;
                else if (roll == 1) currentTask = PossibleTasks.Stockpile;
                else if (roll == 2) currentTask = PossibleTasks.Patrol;
                else if (roll == 3) currentTask = PossibleTasks.Muster;
                else if (roll == 4) currentTask = PossibleTasks.Invest;
                else if (roll == 5) currentTask = PossibleTasks.Trade;
            }
        }
    }

    public void RunTask(string taskName, float delay)
    {
        if(army != null) army.GetComponent<AI_ArmyController>().resetTimer = 0;
        oldSettlement = chosenSettlement;
        arrived = false;

        print(GetComponent<Faction>().faction.ToString() + " Running " + taskName);
        //Console.instance.PrintMessage(GetComponent<Faction>().faction.ToString() + " Running " + taskName);
        StartCoroutine(taskName, delay);
    }

    public IEnumerator Develop(float delay)
    {
        //choose a tile to focus on
        int roll = Random.Range(0, GetComponent<Faction>().ownedTiles.ToArray().Length);
        chosenSettlement = GetComponent<Faction>().ownedTiles[roll].GetComponent<Tile>().settlement;
        //setting the army target to that settlement
        army.GetComponent<Army>().target = chosenSettlement.transform.position;

        yield return new WaitUntil(() => arrived == true); // wait until arrived
        yield return new WaitForSeconds(delay);


        //invoking the change 
        chosenSettlement.GetComponent<Settlement>().province.GetComponent<Tile>().Invoke("IncreaseDevelopment", 0);

        GenerateNextTask();
    }
    public IEnumerator Trade(float delay)
    {
        //choose a tile to focus on
        int roll = Random.Range(0, GetComponent<Faction>().ownedTiles.ToArray().Length);
        chosenSettlement = GetComponent<Faction>().ownedTiles[roll].GetComponent<Tile>().settlement;
        //setting the army target to that settlement
        army.GetComponent<Army>().target = chosenSettlement.transform.position;

        yield return new WaitUntil(() => arrived == true); // wait until arrived
        yield return new WaitForSeconds(delay);


        //invoking the change 
        if(army.GetComponent<Army>().storedItems.ToArray().Length > 0)
        {
            for (int i = 0; i < army.GetComponent<Army>().storedItems.ToArray().Length; i++)
            {
                chosenSettlement.GetComponent<Settlement>().storedItems.Add(army.GetComponent<Army>().storedItems[i]);
                gameObject.GetComponent<Faction>().treasury += army.GetComponent<Army>().storedItems[i].baseValue;
                print(gameObject.name + " SOLD: " + army.GetComponent<Army>().storedItems[i] + " FOR: $" + army.GetComponent<Army>().storedItems[i].baseValue);
            }
            army.GetComponent<Army>().storedItems.Clear();
            GenerateNextTask();
        }
        else
        {
            GenerateNextTask();
            print(gameObject.name + " has no items to trade");
        }
        
    }
    public IEnumerator Patrol(float delay)
    {
        //choose a tile to focus on
        int roll = Random.Range(0, GetComponent<Faction>().ownedTiles.ToArray().Length);
        chosenSettlement = GetComponent<Faction>().ownedTiles[roll].GetComponent<Tile>().settlement;
        //setting the army target to that settlement
        army.GetComponent<Army>().target = chosenSettlement.transform.position;

        yield return new WaitUntil(() => arrived == true); // wait until arrived
        yield return new WaitForSeconds(delay);


        //invoking the change after prolonged time;
        //no change on patrol

        //running the next task
        GenerateNextTask();
    }
    public IEnumerator Muster(float delay)
    {
        if(gameObject.GetComponent<Faction>().taxIncome >= gameObject.GetComponent<Faction>().expenses)
        {
            //choose a tile to focus on
            int roll = Random.Range(0, GetComponent<Faction>().ownedTiles.ToArray().Length);
            chosenSettlement = GetComponent<Faction>().ownedTiles[roll].GetComponent<Tile>().settlement;
            //setting the army target to that settlement
            army.GetComponent<Army>().target = chosenSettlement.transform.position;

            yield return new WaitUntil(() => arrived == true); // wait until arrived
            yield return new WaitForSeconds(delay);


            //invoking the change after prolonged time; Long one this time aye, this one is a coroutine
            chosenSettlement.GetComponent<Settlement>().StartCoroutine(chosenSettlement.GetComponent<Settlement>().Muster(army));
        }
        else
        {
            print(gameObject.name + " can not afford to waste money on more soldiers");
            GenerateNextTask();
        }
        
    }
    public IEnumerator Stockpile(float delay)
    {
        //choose a tile to focus on
        int roll = Random.Range(0, GetComponent<Faction>().ownedTiles.ToArray().Length);
        chosenSettlement = GetComponent<Faction>().ownedTiles[roll].GetComponent<Tile>().settlement;
        //setting the army target to that settlement
        army.GetComponent<Army>().target = chosenSettlement.transform.position;

        yield return new WaitUntil(() => arrived == true); // wait until arrived
        yield return new WaitForSeconds(delay);


        //invoking the change after prolonged time; Long one this time aye, this one is a coroutine
        chosenSettlement.GetComponent<Settlement>().StartCoroutine(chosenSettlement.GetComponent<Settlement>().Stockpile(army));
    }
    public IEnumerator Invest(float delay)
    {
        //choose a tile to focus on
        int roll = Random.Range(0, GetComponent<Faction>().ownedTiles.ToArray().Length);
        chosenSettlement = GetComponent<Faction>().ownedTiles[roll].GetComponent<Tile>().settlement;
        //setting the army target to that settlement
        army.GetComponent<Army>().target = chosenSettlement.transform.position;

        yield return new WaitUntil(() => arrived == true); // wait until arrived
        yield return new WaitForSeconds(delay);


        //invoking the change 
        chosenSettlement.GetComponent<Settlement>().StartCoroutine(chosenSettlement.GetComponent<Settlement>().Invest(gameObject));

        GenerateNextTask();
    }

    public IEnumerator Strategise(float delay)
    {
        if (!GetComponent<Faction>().atWar)
        {
            //choose capital tile to focus on
            chosenSettlement = GetComponent<Faction>().capitalCity;
            //setting the army target to that settlement
            army.GetComponent<Army>().target = chosenSettlement.transform.position;

            yield return new WaitUntil(() => arrived == true); // wait until arrived
            yield return new WaitForSeconds(delay);

            List<GameObject> neighbours = new List<GameObject>();
            //get faction to focus on
            foreach (GameObject tile in GetComponent<Faction>().neighbouringTiles)
            {
                if (!neighbours.Contains(tile.GetComponent<Tile>().ownerObject)) neighbours.Add(tile.GetComponent<Tile>().ownerObject);
            }
            int roll = Random.Range(0, neighbours.ToArray().Length);
            GameObject chosenFaction = neighbours[roll];
            //starting coroutine
            StartCoroutine(ConsiderWar(chosenFaction, 0));
        }
        else //bug fixing
        {
            print(gameObject.name + "ALREADY AT WAR CANNOT DECLARE ANOTHER ONE, FUCKWIT");
            CancelCurrentTask();
        }
        
    }
    public IEnumerator ConsiderWar(GameObject otherFaction, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gameObject.activeInHierarchy)
        {
            float desire = 0; // if desire hits a threshold, than the Ai will declare war

            //testing
            if (GetComponent<Faction>().allies.Contains(otherFaction)) desire -= 1000; // if we are allied
            if (otherFaction.GetComponent<Faction>().atWar) desire += 15; // if they are fighting a war already

            //variables
            //army sizes
            if (otherFaction.GetComponent<Faction>().army.activeInHierarchy)
            {
                desire = difference(GetComponent<Faction>().army.GetComponent<Army>().numOfSoldiers, otherFaction.GetComponent<Faction>().army.GetComponent<Army>().numOfSoldiers, desire, 3);
            }
            else
            {
                desire += 50;
            }
            
            //development
            desire = difference(GetComponent<Faction>().development, otherFaction.GetComponent<Faction>().development, desire, 1.5f);

            if (desire >= 5)
            {
                //announcment
                print(gameObject.name + " DECLARES WAR WITH " + otherFaction.name);
                Console.instance.PrintMessage(gameObject.name + " DECLARES WAR WITH " + otherFaction.name, Color.blue);
                print("with a desire of: " + desire);

                //add diplo modifiers
                GetComponent<Faction>().CommittedAct(GameManager.instance.modifiers[2]); // started agro war

                //logic
                GetComponent<Faction>().atWar = true;
                GetComponent<Faction>().enemies.Add(otherFaction);
                GenerateNextTask();
                taskComplete = true;

                otherFaction.GetComponent<Faction>().atWar = true;
                otherFaction.GetComponent<Faction>().enemies.Add(gameObject);

                if (otherFaction.GetComponent<Faction>().allies.ToArray().Length > 0)
                {
                    foreach(GameObject ally in otherFaction.GetComponent<Faction>().allies)
                    {
                        GetComponent<Faction>().enemies.Add(ally);
                        ally.GetComponent<Faction>().atWar = true;
                        ally.GetComponent<Faction>().enemies.Add(gameObject);

                        if(ally != GameManager.instance.playerFactionObject)
                        {
                            ally.GetComponent<AI_Faction>().CancelCurrentTask();
                            print(ally.name + " CANCELLED BECAUSE A WAR STARTED");
                        }
                    }
                }

                if(GameManager.instance.playerFactionObject != otherFaction)
                {
                    otherFaction.GetComponent<AI_Faction>().CancelCurrentTask();
                    print(otherFaction.name + " CANCELLED BECAUSE A WAR STARTED");
                }
                else
                {
                    DiplomacyTab.instance.SetRelationsToWar(gameObject);
                }
                GameObject newEvent = Instantiate(GameManager.instance.eventPrefab, GameObject.Find("UI").transform);
                newEvent.GetComponent<Event>().title.text = "WAR";
                newEvent.GetComponent<Event>().description.text = GetComponent<Faction>().faction.ToString() + " has declared war with " + otherFaction.GetComponent<Faction>().faction.ToString();
                newEvent.GetComponent<Event>().dismiss.text = "So be it";

                if (otherFaction.GetComponent<Faction>().allies.ToArray().Length > 0)
                {
                    newEvent.GetComponent<Event>().description.text += " and their allies: ";
                    foreach (GameObject ally in otherFaction.GetComponent<Faction>().allies)
                    {
                        newEvent.GetComponent<Event>().description.text += ", " + ally.GetComponent<Faction>().faction.ToString();
                    }
                }
            }
            else
            {
                //announcment
                print(gameObject.name + " DOES NOT WANT WAR WITH " + otherFaction.name);
                print("due to a desire of: " + desire);
            }

            //playing next task after completion
            GenerateNextTask();
            taskComplete = true;
        }
        else print(gameObject.name + " Was too late");
    }
    public IEnumerator Siege(float delay)
    {
        //choose a tile to focus on FORGEIN TILES 
        //choose tile closest to capital
        if (GetComponent<Faction>().atWar)
        {
            int numNeighbours = 0;
            float distance = 1000f;
            List<GameObject> hostileTiles = new List<GameObject>();
            foreach (GameObject tile in GetComponent<Faction>().neighbouringTiles)
            {
                foreach (GameObject enemy in GetComponent<Faction>().enemies)
                {
                    if (tile.GetComponent<Tile>().ownerObject == enemy) hostileTiles.Add(tile);
                }
            }
            if (hostileTiles.ToArray().Length == 0)
            {
                print(gameObject.name + " wont siege as they dont border any hostile tiles");
                CancelCurrentTask();
            }

            //discovering if our capital is in danger
            foreach (GameObject tile in GetComponent<Faction>().capitalCity.GetComponent<Settlement>().province.GetComponent<Tile>().neighbouringTiles)
            {
                if (hostileTiles.Contains(tile))
                {
                    numNeighbours += 1;
                }
            }
            //if not randomly expand
            if (numNeighbours > 0)
            {
                foreach (GameObject tile in hostileTiles)
                {
                    if (Vector2.Distance(tile.transform.position, GetComponent<Faction>().capitalCity.transform.position) < distance)
                    {
                        chosenSettlement = tile.GetComponent<Tile>().settlement;
                        distance = Vector2.Distance(tile.transform.position, GetComponent<Faction>().capitalCity.transform.position);
                    }
                }
                print(gameObject.name + " chosen to attack " + chosenSettlement.GetComponent<Settlement>().settlementName + " because it is only " + distance.ToString() + " From their capital");
            }
            else
            {
                int roll = Random.Range(0, hostileTiles.ToArray().Length);
                chosenSettlement = hostileTiles[roll].GetComponent<Tile>().settlement;
                print(gameObject.name + " is attacking " + chosenSettlement.GetComponent<Settlement>().settlementName + " to enlarge their empire");
            }
            //setting the army target to that settlement
            army.GetComponent<Army>().target = chosenSettlement.transform.position;

            yield return new WaitUntil(() => arrived == true); // wait until arrived
            print("Arrived at Enemy Settlement");
            yield return new WaitForSeconds(0.5f);

            float siegeTime = chosenSettlement.GetComponent<Settlement>().garrisonSize * 4;
            siegeTime -= army.GetComponent<Army>().numOfSoldiers;

            //invoking the change after prolonged time; Long one this time aye, this one is a coroutine
            chosenSettlement.GetComponent<Settlement>().province.GetComponent<Tile>().StartCoroutine(chosenSettlement.GetComponent<Settlement>().province.GetComponent<Tile>().ChangeOwner(gameObject, siegeTime));
        }
        else // Bug Fixing
        {
            print(gameObject.name = " Is not at war and is running a new task");
            CancelCurrentTask();
        }
    }
    public IEnumerator Attack(float delay)
    {
        delay = 0;
        currentTask = PossibleTasks.Attack;
        //choose an army to focus on
        int roll = Random.Range(0, GetComponent<Faction>().enemies.ToArray().Length);
        GameObject chosenArmy = GetComponent<Faction>().enemies[roll].GetComponent<Faction>().army;
        chosenSettlement = chosenArmy; //just roll with it, it doesn't matter

        if (chosenArmy.gameObject.activeInHierarchy == false)
        {
            CancelCurrentTask();
            print(gameObject.name + " cant attack a dead guy lmao --> " + chosenArmy.name);
        }
        //setting the army target to that settlement
        yield return new WaitUntil(() => arrived == true); // wait until arrived
        yield return new WaitForSeconds(delay);



        //task is run on collison with other army
        if(chosenArmy != GameManager.instance.playerFactionObject.GetComponent<Faction>().army)
        {
            if(chosenArmy.GetComponent<Army>().ownerObject.GetComponent<AI_Faction>().currentTask == PossibleTasks.Encounter)
            {
                chosenArmy.GetComponent<Army>().ownerObject.GetComponent<AI_Faction>().CancelCurrentTask();
                print(chosenArmy.name + " decided not to attack as the enemy was already in a battle");
            }
            chosenArmy.GetComponent<Army>().ownerObject.GetComponent<AI_Faction>().StopAllCoroutines();
            chosenArmy.GetComponent<Army>().ownerObject.GetComponent<AI_Faction>().currentTask = PossibleTasks.Encounter;
            chosenArmy.GetComponent<Army>().ownerObject.GetComponent<AI_Faction>().CancelCurrentTask();
            //chosenArmy.GetComponent<Army>().ownerObject.GetComponent<AI_Faction>().chosenSettlement = army;
            army.GetComponent<Army>().Battle(chosenArmy);
        }


        //running the next task
        //this is an exception it is only done when the battle is complete
    }




    //calculating Functions
    public float difference(float a, float b, float product, float multiplier)
    {
        float dif = a - b;
        product += (dif * multiplier);
        return product;
    }

}
