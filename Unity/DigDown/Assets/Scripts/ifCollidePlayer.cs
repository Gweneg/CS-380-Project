using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ifCollidePlayer : MonoBehaviour
{
    public ItemPickup aIP;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {        
        if (collision.gameObject.name == "New Dwarf") //if the object collided only with the game object that is called "New Dwarf"
        {
            aIP.shotgunInInventory = true;
            aIP.ammoShotgun = aIP.ammoShotgun + 10;
            Destroy(gameObject);
        }
    }
}
