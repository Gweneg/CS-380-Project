using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour //gameObject in this case would be the player or the pickaxe, but not my shotgun because shotgun object doesnt have this script
{
    public PlayerMovement aPM; //-forgot what exactly PM is labelled as. Will be used to reference variables from the PlayerMovement script(aka class)
    public GameObject GUNCONTAINER;//parent object
    public GameObject SHOTGUN;//child object

    //public LayerMask childLayerMask;

    public bool Button7ON;
    public Animator anime; //-component variable //-was private
    public int ammoShotgun;
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
        if (Input.GetKeyDown(KeyCode.Alpha7)) //&& shotgun is a child of gunContainer == true) //--//Need to detect key presses only. Once that keypress is pressed, it activates the code once.
        {
            //this code will only make the shotgun in player's hand disappear.
            aPM.holdShotgun = false;
            Debug.Log("Pickaxe equipped");
            anime.SetBool("HoldShotgun", aPM.holdShotgun);
            //shotgun that is a child of dwarf is the one that must disappear.
            SHOTGUN.SetActive(false); //ONLY hide the shotgun that is in players hand
        }
        if (Input.GetKeyDown(KeyCode.Alpha8) && shotgunInInventory == true) //--//Need to detect key presses only. Once that keypress is pressed, it activates the code once.
        {
            aPM.holdShotgun = true;
            Debug.Log("Shotgun equipped");
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

    public void OnTriggerEnter2D(Collider2D collision) //This plays when the object collides with something with an active collider trigger. 
    {
        if (collision.tag == "Player")
        {
            Destroy(gameObject);
            Debug.Log(gameObject + " was Picked up");
        }
        if (collision.tag == "Weapon")
        {
            anime.SetTrigger("Pickaxe");
            anime.SetBool("Pickaxe (Hold)", true);
            Debug.Log(gameObject + " picked up weapon");
        }
    }
}
