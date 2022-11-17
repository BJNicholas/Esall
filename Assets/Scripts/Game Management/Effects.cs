using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public static library of effect prefabs to spawn in other scripts
public class Effects : MonoBehaviour
{
    public static Effects instance;

    public GameObject fire;
    public GameObject travel;

    private void Awake()
    {
        instance = this;
    }


    public void SpawnEffect(GameObject effect, Vector3 spawnPoint)
    {
        GameObject newEffect = Instantiate(effect, spawnPoint, Quaternion.identity);

        StartCoroutine(DestroyEffect(newEffect, newEffect.GetComponent<ParticleSystem>().main.duration + 10));
    }

    public IEnumerator DestroyEffect(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroyImmediate(effect, true);
    }
}
