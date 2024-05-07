using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerMovement : MonoBehaviour
{
    public bool holdShotgun = false;
    [SerializeField] private float speed;
    private Rigidbody2D body; //-variable to reference the RigidBody component
    public Animator anime; //- was private
    private bool grounded;

    // Awake is called at the start
    public void Awake() //- check what exactly this function does
    {
        body = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>(); 
    }

    void Start()
    {
    
    UnityEngine.Debug.Log("PlayerMovement class called");
    }

    //-Update function is called in every frame
    public void Update()
    {
        //handles walking left to right
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        //flips the sprite based on if walking left or right
        if(horizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(.25f, .25f, .25f);//- move right
        } else if(horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-.25f, .25f, .25f);//-move left
        }

        //Checks for a space bar input and if our player is standing on a ground for a jump action
        if ((Input.GetKey(KeyCode.Space) && grounded) && holdShotgun)
        {
            anime.SetBool("HoldShotgun", holdShotgun);
            JumpWShotgun();
        } else if(Input.GetKey(KeyCode.Space) && grounded)
        {
            Jump();
        }

        //starts a running animation
        anime.SetBool("Run", horizontalInput != 0); 
        //lets the animations know the player is on the ground
        anime.SetBool("Grounded", grounded); //-Grounded parameter automatically changes. At all times it'll either be grounded or not grounded.
    }

    //handles the Jump command
    private void Jump()
    {
        //executes the jump
        body.velocity = new Vector2(body.velocity.x, speed);
        //plays the jump animation
        anime.SetTrigger("Jump");
        grounded = false;
    }
    private void JumpWShotgun() //- custom addition
    {
        body.velocity = new Vector2(body.velocity.x, speed);
        anime.SetTrigger("JumpShotgun");
        grounded = false;
    }

    //checks if player is on the ground
    public void OnCollisionEnter2D(Collision2D collision) //- was blank before
    {
        if(collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Enemy") 
        {
            grounded = true;
        }
    }
    //checks to see if the player is capable of attacking
    public bool canAttack()
    {
        return true;
    }
}
