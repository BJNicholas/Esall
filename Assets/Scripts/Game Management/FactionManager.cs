using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionManager : MonoBehaviour
{
    public static FactionManager instance;
    public enum factions
    {
        Observer,
        //Empire
        Esallia,
        //"Greeks"
        Daifax,
        Ostres,
        Palepaes,
        Gallifraes,
        Eopicis,
        //"German/ druidic tribe people"
        Gaulis,
        Allemanne,
        //"Arab-moors / Beduin people"
        Unand,
        Ukin,
        Chohari
    }
    public GameObject[] factionObjects;

    private void Awake()
    {
        instance = this;
        foreach(GameObject faction in factionObjects)
        {
            faction.GetComponent<Faction>().capitalCity.GetComponent<Settlement>().capital = true;
            faction.GetComponent<Faction>().id = System.Array.IndexOf(factionObjects, faction);
        }
    }

}
