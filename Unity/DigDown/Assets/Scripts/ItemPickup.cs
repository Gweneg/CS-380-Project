using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private Animator anime; //-component variable
    // Start is called before the first frame update
    void Start()
    {
        anime = GetComponent<Animator>(); //-you can use this to call the triggers in the animator
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision) //-check what exactly this function does.
    {
        if (collision.tag == "Player")
        {
            Destroy(gameObject);
            Debug.Log(gameObject + " was Picked up");
        }
        if (collision.tag == "Shotgun")
        {
            anime.SetBool("HoldShotgun", true);
            Debug.Log("the current game object is" + gameObject);

        }
        if (collision.tag == "Weapon")
        {
            anime.SetBool("HoldShotgun", false);
            anime.SetTrigger("Pickaxe");

            anime.SetBool("Pickaxe (Hold)", true);
            Debug.Log(gameObject + " picked up weapon");
        }
    }
}
