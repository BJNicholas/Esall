using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Culture : MonoBehaviour
{
    public CultureManager.cultures culture;

    public Color32 colour;

    public string[] names;

    public Sprite portrait; // add more later

    public List<GameObject> cultureTiles;


    private void Awake()
    {
        foreach(GameObject tile in cultureTiles)
        {
            tile.GetComponent<Tile>().culture = culture;
        }
    }
}
