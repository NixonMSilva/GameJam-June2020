﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame ()
    {
        SceneManager.LoadScene("Controls");
    }

    public void StartGame ()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ExitGame ()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void BackToMenu ()
    {
        SceneManager.LoadScene("Intro");
    }
}
