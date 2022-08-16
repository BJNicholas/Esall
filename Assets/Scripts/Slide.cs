using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide : MonoBehaviour
{
    public KeyCode interactionKey;

    private void Update()
    {
        if (Input.GetKeyDown(interactionKey)) KeyAction();
    }


    public void KeyAction()
    {
        if (gameObject.activeInHierarchy)
        {
            Tutorial.instance.NextSlide();
        }
        else
        {
            print("Already done that one");
        }
    }
}
