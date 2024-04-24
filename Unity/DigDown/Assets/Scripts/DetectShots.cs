using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DetectShots : MonoBehaviour
{
    public ItemPickup aIP;
    public PlayerMovement aPM;

    public bool inRange; //when i set this to false, the technique does not work. Not sure why.
    public bool shotgunFired;
    public bool enemiesInSight;
    public GameObject[] enemiesInRange;
    public Collider2D[] enemiesInTheRange;
    public GameObject closestEnemy;

    // Start is called before the first frame update
    public void Awake()
    {
        //enemiesInRange = new GameObject[30];
    }

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