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
        PrintMessage(GameManager.instance.playerFaction.ToString() + " has capitulated everyone", Color.blue);
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
