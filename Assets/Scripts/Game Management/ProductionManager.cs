using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionManager : MonoBehaviour
{
    public Item resource;
    public List<GameObject> producingTiles;
    public int baseProductionRate;

    private void Start()
    {
        Produce();
        foreach (GameObject tile in producingTiles)
        {
            tile.GetComponent<Tile>().settlement.GetComponent<Settlement>().producedItems.Add(resource);
        }
    }

    public void Produce()
    {
        foreach(GameObject tile in producingTiles)
        {
            int temp = Mathf.RoundToInt(baseProductionRate + (0.1f * tile.GetComponent<Tile>().development));
            while(temp > 0)
            {
                tile.GetComponent<Tile>().settlement.GetComponent<Settlement>().storedItems.Add(Instantiate(resource));
                temp -= 1;
            }
        }
    }
}
