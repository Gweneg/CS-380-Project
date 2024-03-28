using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour{
    //Work on making the back button be able to go back to the pause menu instead of main menu
    public String PrevScene = "";
    public void SetPrevScene(string sceneName){
        PrevScene = sceneName;
        PlayerPrefs.SetString("PrevScene", sceneName);
    }
    //public AudioMixer audioMixer;
    public void SetVolume(float volume){
        Debug.Log(volume);
        //To do: Get audio and use the following commands
        //audioMixer.SetFloat("Volume",volume);
    }

    public void SetQuality(int qualityIndex){
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    
    public void BacktoMenu(){
        if(PlayerPrefs.GetString("PrevScene") == "Pause Menu"){
            SceneManager.LoadSceneAsync("Pause Menu");
        } else {
            SceneManager.LoadSceneAsync("Main Menu");
        }
        
    }
}
