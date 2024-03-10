using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthBehaviour : MonoBehaviour{
    // Start is called before the first frame update
    public Image health;
    public float totalHealth = 100f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        if(totalHealth <= 0){
            SceneManager.LoadSceneAsync("Dead screen");
        }

        if(Input.GetKeyDown(KeyCode.Return)){
            takeDamage(20);
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            heal(5);
        }
    }
    
    public void takeDamage(float damage){
        totalHealth -= damage;
        health.fillAmount = totalHealth/100f;
    }

    public void heal(float healTotal){
        totalHealth += healTotal;
        totalHealth = Mathf.Clamp(healTotal,0,100);
        health.fillAmount = totalHealth/100f;
    }
}
