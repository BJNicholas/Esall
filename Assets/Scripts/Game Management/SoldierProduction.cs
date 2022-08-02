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
            while (temp > 0)
            {
                tile.GetComponent<Tile>().settlement.GetComponent<Settlement>().availableSoldiers.Add(type);
                temp -= 1;
            }
        }
    }
}