using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Continue();
        }
    }

    public void Continue()
    {
        gameObject.SetActive(false);
        Time.timeScale = GameManager.instance.timeSpeed;
    }
    public void Save()
    {
        GameManager.instance.Save();
    }
    public void Load()
    {
        GameManager.instance.Load();
    }

    public void MainMenu()
    {
        Destroy(GameObject.Find("Audio Manager"));
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }
    public void EndGame()
    {
        Application.Quit();
    }
}
