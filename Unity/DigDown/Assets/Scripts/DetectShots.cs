using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DetectShots : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
       
    }
    public void OnTriggerEnter2D(Collider2D collision) //- was private
    {
        if (collision.tag == "Enemy")
        {
            Debug.Log("an enemy is in range");
        }
    }
    public void OnTriggerExit2D(Collider2D collision) //method for when collision is no longer in contact, when it "exits"
    {
        if (collision.tag == "Enemy") //if there is no longer a collision on the enemy...
        {
            Debug.Log("no enemy in range"); //SUCCESS
        }
    }
}