using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthBehaviour : MonoBehaviour{
    // Start is called before the first frame update
    public Image health;
    public float totalHealth;
    void Start(){
        // Score and health has to reset
        totalHealth = 100f;
    }

    // Update is called once per frame
    void Update(){
        if(totalHealth <= 0){
            SceneManager.LoadSceneAsync("Dead screen");
        }

        if(Input.GetKeyDown(KeyCode.Return)){
            TakeDamage(20);
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            Heal(5);
        }
    }
    
    public void TakeDamage(float damage){
        totalHealth -= damage;
        health.fillAmount = totalHealth/100f;
    }

    public void Heal(float healTotal){
        totalHealth += healTotal;
        totalHealth = Mathf.Clamp(healTotal,0,100);
        health.fillAmount = totalHealth/100f;
    }

    //Optional: Make method displaying score in game
    //Optional: make method that tallys up points depending on enemies killed or items collected
}
