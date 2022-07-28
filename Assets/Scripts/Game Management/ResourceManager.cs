using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;
    public Item[] items;

    private void Awake()
    {
        instance = this;
    }
}
