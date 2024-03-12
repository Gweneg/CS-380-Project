using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuOptions : MonoBehaviour{
    public void LoadGame(){
        SceneManager.LoadSceneAsync("Mines");
    }
    public void OptionsMenu(){
        SceneManager.LoadSceneAsync("Options Menu");
    }
    public void QuitGame(){
        Application.Quit();
    }
}
