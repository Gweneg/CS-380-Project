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
        if (collision.gameObject.name == "New Dwarf") //this lowkey worked
        {
            aIP.shotgunInInventory = true;
            aIP.ammoShotgun = aIP.ammoShotgun + 10;

            //Debug.Log("Shotgun ammo obtained. Ammo value is now " + aIP.ammoShotgun);
            //Debug.Log("this means shotgun only disappeared once ONLY player touched it.");
            Destroy(gameObject);
        }
    }
}
