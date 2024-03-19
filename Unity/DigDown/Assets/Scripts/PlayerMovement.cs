using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;


public class PlayerMovement : MonoBehaviour //-PlayerMovement is name of the script and class

{
    [SerializeField] private float speed;
    private Rigidbody2D body; //-variable to reference the RigidBody component
    private Animator anime; //-variable to reference the Animator component
    private bool grounded;
    // Awake is called at the start
    private void Awake(){ //- check what exactly this function does
        body = GetComponent<Rigidbody2D>(); //-check what this means
        anime = GetComponent<Animator>(); //-check what this means
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
            transform.localScale = new Vector3(.25f, .25f, .25f);
        } else if(horizontalInput < -0.01f){
            transform.localScale = new Vector3(-.25f, .25f, .25f);
        }
        //Checks for a space bar input and if our player is standing on a ground for a jump action
        if(Input.GetKey(KeyCode.Space) && grounded){
            anime.SetBool("HoldShotgun", false);//- my addition
            Jump();
        }
        //starts a running animation
        anime.SetBool("Run", horizontalInput != 0);
        //lets the animations know the player is on the ground
        anime.SetBool("Grounded", grounded);
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
