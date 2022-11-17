using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    public bool showMerchants;

    private void Awake()
    {
        instance = this;
    }

    public void ToggleMerchants()
    {
        if(showMerchants == true)
        {
            showMerchants = false;
            return;
        }
        else
        {
            showMerchants = true;
            return;
        }
    }


}
