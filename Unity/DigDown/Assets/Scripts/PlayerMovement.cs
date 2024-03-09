using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D body;
    private Animator anime;
    private bool grounded;
    // Awake is called at the start 
    private void Awake(){
        body = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
    }
    // Update is called when input is recieved
    private void Update(){
        //handles walking left to right
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        //flips the sprite based on if walking left or right
        if(horizontalInput > 0.01f){
            transform.localScale = new Vector3(.5f, .5f, .5f);
        } else if(horizontalInput < -0.01f){
            transform.localScale = new Vector3(-.5f, .5f, .5f);
        }
        //Checks for a space bar input and if our player is standing on a ground for a jump action
        if(Input.GetKey(KeyCode.Space) && grounded){
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
    public bool canAttack(){
        return true;
    }
}
