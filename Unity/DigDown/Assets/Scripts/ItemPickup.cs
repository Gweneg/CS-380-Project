using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public PlayerMovement aPM; //-forgot what exactly PM is labelled as. Will be used to reference variables from the PlayerMovement script(aka class)
    public GameObject GUNCONTAINER;//parent object
    public GameObject SHOTGUN;//child object

    //public LayerMask childLayerMask;

    public Animator anime; //-component variable //-was private
    public int ammoShotgun;
    public bool pickaxeInInventory;
    public bool shotgunInInventory;
    public bool holdPickaxe = false;

    // Start is called before the first frame update
    public void Start()
    {
        SHOTGUN = GameObject.Find("Shotgun");
        GUNCONTAINER = GameObject.Find("gunContainer");
        anime = GetComponent<Animator>(); //-you can use this to call the triggers in the animator
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && pickaxeInInventory == true) 
        {
            holdPickaxe = true;
            Debug.Log("Pickaxe equipped");
            //anime.SetTrigger("Pickaxe");
            anime.SetBool("Pickaxe (Hold)", true);
            aPM.holdShotgun = false;
            anime.SetBool("HoldShotgun", aPM.holdShotgun);
            SHOTGUN.SetActive(false); //ONLY hide the shotgun that is in players hand
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && shotgunInInventory == true)
        {
            holdPickaxe = false;
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
}
