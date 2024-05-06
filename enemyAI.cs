using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class enemyAi : MonoBehaviour
{
    //enemy movement and health 
    public float speed = 10f;
    public float stoppingDistance = 1f;
    public float distanceToPlayer;
    public float jump = 1f;
    public float HP = 3;
    public float attackRange = 1f;
    public float attackDamage = .1f;
    public float knockBackForce = 1f;
    public float coolDownTimer = 3f;
    public float lastTimeAttack;



    private Vector3 dir = Vector3.left;
    private Rigidbody2D body;
    private Animator anime;

    public LayerMask groundLayer;
    //public LayerMask enemyLayers;

    public GameObject obstacle; //future rename to objects
    public GameObject enemy;
    public GameObject player;

    //public Transform target;

    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    public bool followPlayer = false;
    public bool isGrounded;
    public bool isJumping;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        rb = GetComponent<Rigidbody2D>();
        groundLayer = LayerMask.GetMask("Ground");
        obstacle = GameObject.FindGameObjectWithTag("obstacle");
        enemy = GameObject.FindGameObjectWithTag("enemy");
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
    //flip sprite
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
    }
    void Update()
    {
        //flip sprite
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        if (horizontalInput > 0.01f)
        {
            dir = Vector3.right;
        }
        else if (horizontalInput < -0.01f)
        {
            dir = Vector3.left;
        }

        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, groundLayer);
        lastTimeAttack = -coolDownTimer;

        //if player is detected, then follow the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= stoppingDistance)
        {
            followPlayer = true;
        }
        else
        {
            followPlayer = false;
        }

        if (followPlayer)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            transform.position += speed * Time.deltaTime * direction;
        }
        //if not follwing the player, patrol
        if (!followPlayer)
        {
            Patrol();
        }
        //if the player is within the circle collider, attack
        if (Time.time - lastTimeAttack >= coolDownTimer)
        {

            if (distanceToPlayer <= attackRange)
            {
                Attack();
                anime.SetTrigger("Attack");
                lastTimeAttack = Time.time;
            }
        }
        //if the player damages the enemy to HP = 0, die
        if (HP < 1)
        {
            Die();
            anime.SetTrigger("Die");
        }
        anime.SetBool("IsWalking", Mathf.Abs(horizontalInput) > 0.01f);
        
    }
    void Jump()
    {
        anime.SetTrigger("Jump");
        rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
        isJumping = true;


        if (!isGrounded)
        {

            rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
            isJumping = true;
            //test to check ground
            isGrounded = false;
        }

    }

    //if come into contact with another enemy, turn around
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("enemy"))
        {
            if (dir == Vector3.left)
            {
                dir = Vector3.right;
            }
            else
            {
                dir = Vector3.left;
            }
        }
        //if circle collider detects obstacle, jump
        if (collision.CompareTag("obstacle"))
        {
            Jump();
        }

        if (collision.CompareTag("enemy"))
        {
            HP = HP - .1f;
        }

    }


    void Patrol()
    {
        transform.Translate(speed * Time.deltaTime * dir);
        if (transform.position.x <= -3)
        {
            dir = Vector3.right;
        }
        else if (transform.position.x >= 3)
        {
            dir = Vector3.left;
        }
        // flip sprite
        if (dir == Vector3.left)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

    }

    void Attack()
    {
        //anime.SetTrigger("Attack");
        Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
        player.GetComponent<Rigidbody2D>().AddForce(knockbackDirection * knockBackForce, ForceMode2D.Impulse);
    }
    
    void Die()
    {
        //anime.SetTrigger("Die");
        Destroy(gameObject);
    }
    
}
