using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour //gameObject in this case would be the player or the pickaxe, but not my shotgun because shotgun object doesnt have this script
{
    public PlayerMovement aPM; //-forgot what exactly PM is labelled as. Will be used to reference variables from the PlayerMovement script(aka class)
    public GameObject GUNCONTAINER;//parent object
    public GameObject SHOTGUN;//child object

    //public LayerMask childLayerMask;

    public Animator anime; //-component variable //-was private
    public int ammoShotgun;
    public bool pickaxeInInventory;
    public bool shotgunInInventory;

    // Start is called before the first frame update
    public void Start()
    {
        SHOTGUN = GameObject.Find("Shotgun");
        GUNCONTAINER = GameObject.Find("gunContainer");
        anime = GetComponent<Animator>(); //-you can use this to call the triggers in the animator
    }

    // Update is called once per frame

    /////////////////////////////////////////////////////////////////////////
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7) && pickaxeInInventory == true) 
        {
            Debug.Log("Pickaxe equipped");
            //anime.SetTrigger("Pickaxe");
            anime.SetBool("Pickaxe (Hold)", true);
            aPM.holdShotgun = false;
            anime.SetBool("HoldShotgun", aPM.holdShotgun);
            SHOTGUN.SetActive(false); //ONLY hide the shotgun that is in players hand
        }
        if (Input.GetKeyDown(KeyCode.Alpha8) && shotgunInInventory == true)
        {
            Debug.Log("Shotgun equipped");
            anime.SetBool("Pickaxe (Hold)", false);
            aPM.holdShotgun = true;
            anime.SetBool("HoldShotgun", aPM.holdShotgun);
            //make the shotgun object reappear, move shotgun to the gun container so it appears character is holding the shotgun
            SHOTGUN.SetActive(true);
            SHOTGUN.transform.SetParent(GUNCONTAINER.transform); //this makes the shotgun object be set as a child for the guncontainer and dwarf sprite. So that now it will flip sides with the dwarf.
            SHOTGUN.transform.localPosition = Vector3.zero;//These 3 lines help maintian the shotgun gameObject's position/rotation with the guncontainer game object
            SHOTGUN.transform.localRotation = Quaternion.identity;//^^^
            SHOTGUN.transform.localScale = Vector3.one;//^^^
        }
    }
    /////////////////////////////////////////////////////////////////////////

    //public void OnTriggerEnter2D(Collider2D collision) //This plays when the object collides with something with an active collider trigger. 
    //{
    //    if (collision.tag == "Player")
    //    {
    //        Destroy(gameObject);
    //        Debug.Log(gameObject + " was Picked up");
    //    }
    //    if (collision.tag == "Weapon")
    //    {
    //        anime.SetTrigger("Pickaxe");
    //        anime.SetBool("Pickaxe (Hold)", true);
    //        Debug.Log(gameObject + " picked up weapon");
    //    }

    //    //if (collision.gameObject.name == "New Dwarf") //add this code to a separate script in the Pickaxe game object. Also delete the OnTriggerEnter2D method and code as well.
    //    //{
    //    //    anime.SetTrigger("Pickaxe");
    //    //    anime.SetBool("Pickaxe (Hold)", true);
    //    //    //Debug.Log(gameObject + " picked up weapon");
    //    //    Destroy(gameObject);
    //    //    Debug.Log(gameObject + " was Picked up");
    //    //}
    //}
}
