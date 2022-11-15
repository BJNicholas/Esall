using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public static Console instance;

    public GameObject newMessagePrefab;
    public GameObject contentDisplay;
    public InputField inputField;
    [Space]
    public List<GameObject> messages;
    private string input;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void RunCommand()
    {
        input = inputField.text;
        print(input);
        StartCoroutine(input);

        if(input != "")
        {
            PrintMessage("NOT A VAILD COMMAND", Color.red);
        }

    }

    void ResetInput()
    {
        inputField.text = "";
        input = "";
    }

    public IEnumerator pause()
    {
        ResetInput();
        yield return new WaitForEndOfFrame();
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
            PrintMessage("GAME UNPAUSED", Color.green);
        }
        else if(Time.timeScale != 0)
        {
            Time.timeScale = 0;
            PrintMessage("GAME PAUSED", Color.blue);
        }
    }
    public IEnumerator capall()
    {
        ResetInput();
        yield return new WaitForEndOfFrame();
        PrintMessage(GameManager.instance.playerFaction.ToString() + " has capitulated everyone", Color.magenta);
        foreach(GameObject faction in FactionManager.instance.factionObjects)
        {
            if(faction.GetComponent<Faction>().faction != GameManager.instance.playerFaction)
            {
                faction.GetComponent<Faction>().Capitulate(GameManager.instance.playerFactionObject);
            }
        }
    }

    public IEnumerator allwar()
    {
        ResetInput();
        yield return new WaitForEndOfFrame();
        PrintMessage("ARMAGEDON!!!!", Color.magenta);
        foreach (GameObject faction in FactionManager.instance.factionObjects)
        {
            faction.GetComponent<Faction>().atWar = true;

            foreach (GameObject otherFaction in FactionManager.instance.factionObjects)
            {
                if(otherFaction != faction)
                {
                    faction.GetComponent<Faction>().enemies.Add(otherFaction);
                }
            }
        }
    }

    public IEnumerator happypeeps()
    {
        ResetInput();
        yield return new WaitForEndOfFrame();
        PrintMessage("everyone in your nation now loves you", Color.magenta);
        PrintMessage("ahhhh lovely", Color.magenta);
        foreach (GameObject tile in GameManager.instance.playerFactionObject.GetComponent<Faction>().ownedTiles)
        {
            tile.GetComponent<Tile>().publicOrder = 100;
        }
    }

    public IEnumerator convertall()
    {
        ResetInput();
        yield return new WaitForEndOfFrame();
        PrintMessage("everyone in your nation is now " + GameManager.instance.playerFactionObject.GetComponent<Faction>().culture.ToString(), Color.magenta);
        foreach (GameObject tile in GameManager.instance.playerFactionObject.GetComponent<Faction>().ownedTiles)
        {
            foreach(GameObject culture in CultureManager.instance.cultureObjects)
            {
                if(culture.GetComponent<Culture>().culture == tile.GetComponent<Tile>().culture)
                {
                    culture.GetComponent<Culture>().cultureTiles.Remove(tile);
                }
            }
            tile.GetComponent<Tile>().culture = GameManager.instance.playerFactionObject.GetComponent<Faction>().culture;
            foreach (GameObject culture in CultureManager.instance.cultureObjects)
            {
                if (culture.GetComponent<Culture>().culture == tile.GetComponent<Tile>().culture)
                {
                    culture.GetComponent<Culture>().cultureTiles.Add(tile);
                }
            }
        }
    }

    public IEnumerator help()
    {
        ResetInput();
        yield return new WaitForEndOfFrame();


        PrintMessage("---------", Color.white);
        PrintMessage("and also help...", Color.magenta);
        PrintMessage("", Color.magenta);
        PrintMessage("convertall", Color.magenta);
        PrintMessage("happypeeps", Color.magenta);
        PrintMessage("allwar", Color.magenta);
        PrintMessage("capall", Color.magenta);


        PrintMessage(" ", Color.magenta);
        PrintMessage("---------", Color.white);
        PrintMessage(" ", Color.magenta);
        PrintMessage("text is cap and space sensitive", Color.magenta);
        PrintMessage("WARNING", Color.magenta);
        PrintMessage("The following are all useable commands...", Color.magenta);


    }



        public void PrintMessage(string message, [Optional] Color colour)
    {
        GameObject newMessage = Instantiate(newMessagePrefab, contentDisplay.transform);
        messages.Add(newMessage);
        //contentDisplay.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 55);
        newMessage.GetComponent<Text>().text = message;
        if(colour != Color.clear)
        {
            newMessage.GetComponent<Text>().color = colour;
        }


        // mess with colour later maybe?

        //destory old ones
        if (messages.ToArray().Length > 15)
        {
            Destroy(messages[0]);
            messages.Remove(messages[0]);
        }
    }



}
