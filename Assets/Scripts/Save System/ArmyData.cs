using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArmyData
{
    public int ownerID;
    public bool isPlayer;
    public bool alive;
    public int[] soldiers;
    public int[] storedItems;
    public float[] target;
    public float[] position;

    public ArmyData (Army army)
    {
        //easy conversion

        isPlayer = army.isPlayer;

        if (army.gameObject.activeInHierarchy) alive = true;
        else alive = false;

        //complex conversion

        //target
        target = new float[3];
        target[0] = army.target.x;
        target[1] = army.target.y;
        target[2] = army.target.z;

        //position
        position = new float[3];
        position[0] = army.gameObject.transform.position.x;
        position[1] = army.gameObject.transform.position.y;
        position[2] = army.gameObject.transform.position.z;

        //owner
        foreach (GameObject faction in FactionManager.instance.factionObjects)
        {
            if (faction.GetComponent<Faction>().id == army.ownerObject.GetComponent<Faction>().id)
            {
                ownerID = faction.GetComponent<Faction>().id;
                break;
            }
            else ownerID = 1000; //observer
        }
        //Soldiers
        soldiers = new int[army.soldiers.ToArray().Length];
        for (int i = 0; i < soldiers.Length; i++)
        {
            soldiers[i] = army.soldiers[i].id;
        }
        //Items
        storedItems = new int[army.storedItems.ToArray().Length];
        for (int i = 0; i < storedItems.Length; i++)
        {
            storedItems[i] = army.storedItems[i].id;
        }
    }
}
