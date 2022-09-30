using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MerchantData
{
    public int homeProvID;
    public int ownerID;
    public float[] target;
    public float[] position;
    public float purchasePrice;
    public float sellValue;
    public int[] storedItems;

    public MerchantData(Merchant merchant)
    {
        //easy conversion

        purchasePrice = merchant.purchasePrice;
        sellValue = merchant.sellValue;

        //complex conversion

        //target
        target = new float[3];
        target[0] = merchant.target.position.x;
        target[1] = merchant.target.position.y;
        target[2] = merchant.target.position.z;

        //position
        position = new float[3];
        position[0] = merchant.gameObject.transform.position.x;
        position[1] = merchant.gameObject.transform.position.y;
        position[2] = merchant.gameObject.transform.position.z;

        //owner
        foreach (GameObject faction in FactionManager.instance.factionObjects)
        {
            if (faction.GetComponent<Faction>().id == merchant.ownerObject.GetComponent<Faction>().id)
            {
                ownerID = faction.GetComponent<Faction>().id;
                break;
            }
            else ownerID = 1000; //observer
        }
        //Items
        storedItems = new int[merchant.storedItems.ToArray().Length];
        for (int i = 0; i < storedItems.Length; i++)
        {
            storedItems[i] = merchant.storedItems[i].id;
        }
        //Home 
        foreach(GameObject tile in GameManager.instance.provinces)
        {
            if(tile.GetComponent<Tile>().settlement == merchant.homeCity)
            {
                homeProvID = tile.GetComponent<Tile>().id;
            }
        }
    }
}
