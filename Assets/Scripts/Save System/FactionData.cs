using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FactionData
{
    public int[] ownedTiles;
    public float treasury;
    public bool atWar;
    public int[] allies, enemies;

    public FactionData (Faction faction)
    {
        //easy conversion

        treasury = faction.treasury;
        atWar = faction.atWar;

        //complex conversion

        //owned
        ownedTiles = new int[faction.ownedTiles.ToArray().Length];
        for(int i = 0; i < ownedTiles.Length; i++)
        {
            ownedTiles[i] = faction.ownedTiles[i].GetComponent<Tile>().id;
        }
        //allies
        allies = new int[faction.allies.ToArray().Length];
        for (int i = 0; i < allies.Length; i++)
        {
            allies[i] = faction.allies[i].GetComponent<Faction>().id;
        }
        //enemies
        enemies = new int[faction.enemies.ToArray().Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = faction.enemies[i].GetComponent<Faction>().id;
        }

    }
}
