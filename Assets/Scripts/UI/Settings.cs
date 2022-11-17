using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    public bool showMerchants;
    public Slider music;

    private void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        GameObject musicObj = GameObject.Find("Audio Manager");
        if (musicObj != null)
        {
            musicObj.GetComponent<AudioSource>().volume = music.value;
        }
        else print("No AudioManager in this scene!");
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
