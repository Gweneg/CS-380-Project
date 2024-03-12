using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private Animator anime;
    // Start is called before the first frame update
    void Start()
    {
        anime = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player"){
            Destroy(gameObject);
            Debug.Log(gameObject + " was Picked up");
        }
        if(collision.tag == "Weapon"){
            anime.SetTrigger("Pickaxe");
            anime.SetBool("Pickaxe (Hold)", true);
            Debug.Log(gameObject + " picked up weapon");
        }
    }
}
