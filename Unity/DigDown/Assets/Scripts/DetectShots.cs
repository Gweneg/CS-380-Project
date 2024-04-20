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

//Method.Make a fixed distance number. 1 foot away from player.
//Whichever enemy that is closest to that distance number when player shoots,
//is the one that gets destroyed. So basically the object with the smaller number distance
//gets destroyed.

//Make a game object variable called closestObject that stores the object that
//has the shortest distance to the player.
//In the end, the closestObject game object is what gets destroyed.

//what may need to happen, is once the shot is fired, that is when the array is made
//this array stores all the objects that were within the shotgun range box collider
//After those objects have been stored in the array, that is when the comparing of distances
//is made to figure out which object was closest to the player. Whichever object that
//was closest will be stored in the closestObject variable, and that variable gets destroyed.
//After that, the array could probably just be emptied.