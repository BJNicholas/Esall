using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevTools : MonoBehaviour
{
    [Header("Simulation")]
    [Range(1, 100)] public float simulationSpeed = 1;


    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        GameManager.instance.timeSpeed = simulationSpeed;
    }
}
