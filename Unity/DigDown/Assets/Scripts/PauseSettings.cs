using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseSettings : MonoBehaviour{
    public SettingsMenu settingsMenuInstance;
    public void ResumeGame(){
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync("Mines");
    }

    public void MainMenu(){
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public void Settings(){
        SceneManager.LoadSceneAsync("Options Menu");
        settingsMenuInstance.SetPrevScene("Pause Menu");
    }
}
