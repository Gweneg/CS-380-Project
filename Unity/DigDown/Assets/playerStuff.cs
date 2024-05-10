using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerStuff : MonoBehaviour
{
    public float HP = 100;

    public GameObject player;
    public GameObject enemy;
    public Rigidbody2D rb;
    public LayerMask groundLayer;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        enemy = GameObject.FindGameObjectWithTag("enemy");
        rb = GetComponent<Rigidbody2D>();
        groundLayer = LayerMask.GetMask("groundLayer");
    }

    // Update is called once per frame
    void Update()
    {
        if (HP < 1)
        {
            Die();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("enemy"))
        {
            HP = HP - .1f;
        }
    }
    void Die()
    {
        Destroy(gameObject);
    }
}
