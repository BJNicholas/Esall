using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeScreen : MonoBehaviour
{
    public GameObject HUD;

    public void ChooseFaction(GameObject chosenFaction)
    {
        GameManager.instance.playerFaction = chosenFaction.GetComponent<Faction>().faction;
        GameManager.instance.playerFactionObject = chosenFaction;
        foreach (GameObject faction in FactionManager.instance.factionObjects)
        {
            faction.GetComponent<Faction>().SpawnNewArmy();
            if (faction != chosenFaction) faction.AddComponent<AI_Faction>();
        }
        Camera.main.gameObject.GetComponent<CameraController>().FindPlayerArmy();
        HUD.SetActive(true);
        gameObject.SetActive(false);

        GameManager.instance.Save();
    }
    public void Observe(GameObject chosenFaction)
    {
        GameManager.instance.playerFaction = chosenFaction.GetComponent<Faction>().faction;
        GameManager.instance.playerFactionObject = chosenFaction;
        foreach (GameObject faction in FactionManager.instance.factionObjects)
        {
            faction.GetComponent<Faction>().SpawnNewArmy();
            faction.AddComponent<AI_Faction>();
        }
        HUD.SetActive(true);
        gameObject.SetActive(false);

        GameManager.instance.Save();
    }
}
