using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour //gameObject in this case would be the player or the pickaxe, but not my shotgun because shotgun object doesnt have this script
{
    public PlayerMovement aPM; //-forgot what exactly PM is labelled as. Will be used to reference variables from the PlayerMovement script(aka class)
    public GameObject GUNCONTAINER;//parent object
    public GameObject SHOTGUN;//child object
    public bool Button7ON;
    public Animator anime; //-component variable //-was private
    public int ammoShotgun;

    // Start is called before the first frame update
    public void Start()
    {
        SHOTGUN = GameObject.Find("Shotgun");
        GUNCONTAINER = GameObject.Find("gunContainer");
        anime = GetComponent<Animator>(); //-you can use this to call the triggers in the animator
    }

    // Update is called once per frame
    //--//--//--//--//--//--//--//--
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7))//--//Need to detect key presses only. Once that keypress is pressed, it activates the code once.
        {
            aPM.holdShotgun = false;
            Debug.Log("Pickaxe equipped");
            anime.SetBool("HoldShotgun", aPM.holdShotgun);
            SHOTGUN.SetActive(false); //hide shotgun gameObject since character would now be holding the pickaxe.
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))//--//Need to detect key presses only. Once that keypress is pressed, it activates the code once.
        {
            Debug.Log("ammoShotgun value is " + ammoShotgun);
            aPM.holdShotgun = true;
            Debug.Log("Shotgun equipped");
            anime.SetBool("HoldShotgun", aPM.holdShotgun);
            //make the shotgun object reappear, move shotgun to the gun container so it appears character is holding the shotgun
            SHOTGUN.SetActive(true);
            SHOTGUN.transform.SetParent(GUNCONTAINER.transform);
            SHOTGUN.transform.localPosition = Vector3.zero;//These 3 lines help maintian the shotgun gameObject's position/rotation with the guncontainer game object
            SHOTGUN.transform.localRotation = Quaternion.identity;//^^^
            SHOTGUN.transform.localScale = Vector3.one;//^^^

        }
    }
    //--//--//--//--//--//--//--//--

    public void OnTriggerEnter2D(Collider2D collision) //-check what exactly this function does. (got a better idea what it means) //- was private
    {
        if (collision.tag == "Player")
        {
            Destroy(gameObject);
            Debug.Log(gameObject + " was Picked up");
        }
        if (collision.tag == "Shotgun" && aPM.holdShotgun == false)
        {
            //on collision, remove shotgun from floor, and add to player inventory
            //-SHOTGUN variable now refers to the Shotgun gameObject.
            SHOTGUN.SetActive(false);
            ammoShotgun = 10;
        }
        if (collision.tag == "Weapon")
        {
            anime.SetTrigger("Pickaxe");
            anime.SetBool("Pickaxe (Hold)", true);
            Debug.Log(gameObject + " picked up weapon");
        }
    }
}
