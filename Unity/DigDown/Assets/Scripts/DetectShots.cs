using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectShots : MonoBehaviour
{
    public bool inRange; //when i set this to false, the technique does not work. Not sure why.


    // Start is called before the first frame update
    public void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter2D(Collider2D collision) //-check what exactly this function does. (got a better idea what it means) //- was private
    {
        if (collision.tag == "Enemy")
        {
            inRange = true;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            inRange = false;
            Debug.Log("this should show up when shotgun is not colliding with enemy."); //SUCCESS
        }
    }

}
