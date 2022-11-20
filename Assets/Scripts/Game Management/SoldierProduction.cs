using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierProduction : MonoBehaviour
{
    public Soldier type;
    public List<GameObject> producingTiles;
    public int baseProductionRate;

    private void Start()
    {
        Produce();
    }

    public void Produce()
    {
        foreach (GameObject tile in producingTiles)
        {
            int temp = baseProductionRate;

            List<Soldier> soldiersOfType = new List<Soldier>();

            foreach(Soldier unit in tile.GetComponent<Tile>().settlement.GetComponent<Settlement>().availableSoldiers)
            {
                if (unit.name == type.name) soldiersOfType.Add(unit);
            }

            while (temp > 0 && soldiersOfType.ToArray().Length <= tile.GetComponent<Tile>().manpowerCap)
            {
                tile.GetComponent<Tile>().settlement.GetComponent<Settlement>().availableSoldiers.Add(Instantiate(type));
                //print("produced a " + type + " in " + tile.GetComponent<Tile>().settlement);
                temp -= 1;
            }
        }

        if (RecruitmentHub.instance.gameObject.activeInHierarchy)
        {
            RecruitmentHub.instance.LiveUpdateListings();
        }
    }
}
