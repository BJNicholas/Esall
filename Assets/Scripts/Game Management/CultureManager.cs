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

    private void Awake()
    {
        instance = this;
    }
}
