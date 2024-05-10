using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ifPickaxeCollidePlayer : MonoBehaviour
{
    public ItemPickup aIP;
    // Start is called before the first frame update
    void Start()
    {
        aIP.anime = aIP.anime.GetComponent<Animator>(); //I assume this refers to the component in general.
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "New Dwarf")
        {
            aIP.pickaxeInInventory = true;
            //aIP.anime.SetTrigger("Pickaxe");
            //aIP.anime.SetBool("Pickaxe (Hold)", true);
            //Debug.Log(gameObject + " picked up weapon");
            Destroy(gameObject);
            Debug.Log(gameObject + " was Picked up");
        }
    }
}
