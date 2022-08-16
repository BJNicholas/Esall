using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;
    public List<GameObject> slides;
    public GameObject currentSlide;


    private void Start()
    {
        instance = this;
        currentSlide = slides[0];
        foreach(GameObject slide in slides)
        {
            if (slide != currentSlide) slide.SetActive(false);
        }
    }

    public void NextSlide()
    {
        currentSlide.SetActive(false);
        currentSlide = slides[slides.IndexOf(currentSlide) + 1];
        currentSlide.SetActive(true);
    }
}
