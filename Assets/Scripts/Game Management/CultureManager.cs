using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultureManager : MonoBehaviour
{
    public static CultureManager instance;
    public enum cultures
    {
        Latinus,
        Myceanie,
        Safarid,
        Galig,
        Sachser
    }
    public GameObject[] cultureObjects;

    public PossibleEvent conversionEvent;


    public void Conversion(Tile tile, cultures newCulture)
    {
        for (int i = 0; i < cultureObjects.Length; i++)
        {
            if (cultureObjects[i].GetComponent<Culture>().culture == tile.culture)
            {
                for (int y = 0; y < cultureObjects[i].GetComponent<Culture>().cultureTiles.ToArray().Length; y++)
                {
                    if (cultureObjects[i].GetComponent<Culture>().cultureTiles[y] == tile.gameObject)
                    {
                        cultureObjects[i].GetComponent<Culture>().cultureTiles.RemoveAt(y);
                        break;
                    }
                }
            }
            if(cultureObjects[i].GetComponent<Culture>().culture == newCulture)
            {
                cultureObjects[i].GetComponent<Culture>().cultureTiles.Add(tile.gameObject);
            }

        }

        tile.culture = newCulture;

    }

    private void Awake()
    {
        instance = this;
    }
}
