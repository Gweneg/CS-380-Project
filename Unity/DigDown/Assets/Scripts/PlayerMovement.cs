using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

//This class was called first

//-Problem may be the order of which the scripts are ran. When calling PM.holdShotgun in ItemPickup class,
//its value may not be what I'm expecting based on PlayerMovement's script iteration.
public class PlayerMovement : MonoBehaviour //-PlayerMovement is name of the script and class
{
    [SerializeField] private float speed;
    private Rigidbody2D body; //-variable to reference the RigidBody component
    private Animator anime; //-variable to reference the Animator component
    private bool grounded;
    public bool holdShotgun = false;

    // Awake is called at the start
    private void Awake() { //- check what exactly this function does
        body = GetComponent<Rigidbody2D>(); //-check the difference between this line vs a variable of type Rigidbody2D used to change component settings.
        anime = GetComponent<Animator>(); //-check what this means
    }

    void Start()
    {
        UnityEngine.Debug.Log("PlayerMovement class called");
    }

    // Update is called when input is recieved.
    //-Update function is called in every frame
    private void Update(){
        //handles walking left to right
        float horizontalInput = Input.GetAxis("Horizontal"); //-Input.GetAxis("Horizontal") equals either -1 or 1, representing either left or right direction
                                                             //-So horizontalInput would equal either -1 or 1
                                                             //-Value increases as you hold down input, decreases once you let go, back to 0.
        //Debug.Log("horizontal input current value is " + horizontalInput); //-
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        //flips the sprite based on if walking left or right
        if(horizontalInput > 0.01f){
            transform.localScale = new Vector3(.25f, .25f, .25f);//- move right
        } else if(horizontalInput < -0.01f){
            transform.localScale = new Vector3(-.25f, .25f, .25f);//-move left
        }
        //Checks for a space bar input and if our player is standing on a ground for a jump action
        if(Input.GetKey(KeyCode.Space) && grounded && holdShotgun){
            holdShotgun = false; //-plan may be to only hold shotgun while grounded.
            anime.SetBool("HoldShotgun", holdShotgun);//- my addition
            Jump();
        } else if (Input.GetKey(KeyCode.Space) && grounded && holdShotgun == false)
        {
            anime.SetBool("HoldShotgun", holdShotgun);
            Jump();
        }
        //starts a running animation
        anime.SetBool("Run", horizontalInput != 0); //-Run animation automatically plays when horizontalInput is anything but 0. 
        //lets the animations know the player is on the ground
        anime.SetBool("Grounded", grounded); //-Grounded parameter automatically changes. At all times it'll either be grounded or not grounded.
    }
    //handles the Jump command
    private void Jump(){
        //executes the jump
        body.velocity = new Vector2(body.velocity.x, speed);
        //plays the jump animation
        anime.SetTrigger("Jump");
        grounded = false;
    }
    //checks if player is on the ground
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground"){
            grounded = true;
        }
    }
    //checks to see if the player is capable of attacking
    //-
    public bool canAttack(){
        return true;
    }
}
