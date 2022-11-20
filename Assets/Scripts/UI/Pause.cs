using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public static Pause instance;
    public float oldSpeedValue = 0;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void PauseGame()
    {
        oldSpeedValue = GameManager.instance.timeSpeed;
        //GameManager.instance.timeSpeed = 0f;

        Time.timeScale = 0f;

        gameObject.SetActive(true);
    }

    public void Continue()
    {
        GameManager.instance.timeSpeed = oldSpeedValue;
        gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
    //public void Save()
    //{
    //    GameManager.instance.Save();
    //}
    //public void Load()
    //{
    //    GameManager.instance.Load();
    //}

    public void MainMenu()
    {
        Destroy(GameObject.Find("Audio Manager"));
        SceneManager.LoadScene(0);
    }
    public void EndGame()
    {
        Application.Quit();
    }
}
