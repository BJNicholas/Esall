using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event : MonoBehaviour
{
    public Text title, description, dismiss;


    private void Start()
    {
        Invoke("Dismiss", 15f);
    }



    public void Dismiss()
    {
        if(gameObject.activeInHierarchy) Destroy(gameObject);
    }
}
