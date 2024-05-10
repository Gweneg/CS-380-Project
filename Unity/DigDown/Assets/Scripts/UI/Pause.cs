using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour{
    public void PauseGame(){
        //Stop time in game
        Time.timeScale = 0;

        //Load Pause Menu
        SceneManager.LoadSceneAsync("Pause Menu");
    }
}
