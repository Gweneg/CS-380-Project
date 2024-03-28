using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour //gameObject in this case would be the player or the pickaxe, but not my shotgun because shotgun object doesnt have this script
{
    //-PlayerMovement PM = new PlayerMovement();
    public GameObject SHOTGUN;
    PlayerMovement PM; //-forgot what exactly PM is labelled as. Will be used to reference variables from the PlayerMovement script(aka class)
    private Animator anime; //-component variable

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Debug.Log("ItemPickup class called");
        //anime = GetComponent<Animator>(); //-you can use this to call the triggers in the animator
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha7))//--
        {
            Debug.Log("equipped pickaxe");
            PM.holdShotgun = false;
            anime.SetBool("HoldShotgun", PM.holdShotgun);
            //-turn on Jesse's "equip pickaxe" animation
        }
        else if (Input.GetKey(KeyCode.Alpha8))
        {
            Debug.Log("equipped shotgun");
            PM.holdShotgun = true;
            anime.SetBool("HoldShotgun", PM.holdShotgun);
            //Debug.Log("the current game object is" + gameObject);//- this would just say New Dwarf
        }//--
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
            //on collision, remove shotgun from floor, and add to player inventory
            SHOTGUN = GameObject.Find("Shotgun"); //-SHOTGUN variable now refers to the Shotgun gameObject.
            Destroy(SHOTGUN);

        }
        if (collision.tag == "Weapon")
        {
            anime.SetBool("HoldShotgun", PM.holdShotgun);
            anime.SetTrigger("Pickaxe");

            anime.SetBool("Pickaxe (Hold)", true);
            Debug.Log(gameObject + " picked up weapon");
        }
    }
}
