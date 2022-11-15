using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarIcon : MonoBehaviour
{
    public Image left, right;

    private void FixedUpdate()
    {
        if(GameManager.instance.playerFactionObject.GetComponent<Faction>().enemies.ToArray().Length > 0)
        {
            left.color = Color.red;
            right.color = Color.red;
        }
        else
        {
            left.color = Color.white;
            right.color = Color.white;
        }
    }
}
