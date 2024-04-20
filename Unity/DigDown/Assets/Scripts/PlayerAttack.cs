using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Il2Cpp;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public ItemPickup aIP; //-reference variable to the ItemPickup class
    public PlayerMovement aPM; //-reference variable to the ItemPickup class - important, keep watch if this works well
    public DetectShots aDS; //-reference variable to the DetectShot class

    //float speed = 20f;
    //float distance = 5f;

    [SerializeField] private float attackCoolDown;
    private Animator anime;

    public RaycastHit2D hit;

    public GameObject ENEMY;
    public GameObject SHOTGUN; //game object variable
    public GameObject SHOTGUN_AUDIO_SOURCE; //**
    public Animator shotgunAnimator; //component variable

    public AudioSource shotgunAudioSource;//component variable
    public AudioClip explosionAudioClip;//component varibale - assign the audio clip you want to play to the explosionAudioClip field in the Inspector

    private PlayerMovement playerMovement; //-reference variable to the ItemPickup class
    private float coolDownTimer = Mathf.Infinity;
    public Transform attackPoint;
    public float attackRange = .5f;
    //public bool shotgunFired;
    
    public LayerMask enemyLayers;

    public BoxCollider2D shotgunRangeCollider; //HERE
    public Collider2D[] enemiesInRangeArr;//HERE

    public Vector2 shotgunColliderCenPos;//HERE
    public Quaternion shotgunColliderRotation;//HERE
    public float angleInDegrees;//HERE
    public Vector2 shotgunCollidersize; //HERE

    //oo GOAL: make a rectangle for the OverLapBox, then make it visible.
    public Vector2 centerPosition;//oo
    public float rotation; //oo


    

    // Awake is called before the first frame update
    private void Awake()
    {
        ENEMY = GameObject.Find("enemy1");

        SHOTGUN = GameObject.Find("Shotgun");//gained access to the SHOTGUN gameobject, and from there I had access to its components, hence shotgunAnimator = SHOTGUN.GetComponent<Animator>();
        shotgunAnimator = SHOTGUN.GetComponent<Animator>();
        anime = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        SHOTGUN_AUDIO_SOURCE = GameObject.Find("shotgunAudioObject"); //**
        shotgunAudioSource = SHOTGUN_AUDIO_SOURCE.GetComponent<AudioSource>();//**
        shotgunAudioSource.clip = explosionAudioClip;//


        shotgunRangeCollider = SHOTGUN.GetComponent<BoxCollider2D>(); //HERE
        shotgunCollidersize = shotgunRangeCollider.size; //we obtained the size of the shotgun's 2D box collider.
        

        Debug.Log("The shotgun's 2D box collider size is " + shotgunCollidersize); //CORRECT SIZE GETS OUTPUTTED.
        Debug.Log("The shotgun's 2D box collider position is " + shotgunColliderCenPos);
    }
    // Update registers user input
    public void Update() //- was private
    {
        //so far so good with the collider positioning.

        if (Input.GetKeyDown(KeyCode.K) && aIP.ammoShotgun > 0 && aPM.holdShotgun)
        {
            //aDS.shotgunFired = true;
            shotgunAnimator.Play("Shotgun Firing", 0, 0f);//**Everytime this condition is met v, the shoot animation should restart. SUCCESS
            shotgunAudioSource.Play();//play the shotgun shoot audio sound
            aIP.ammoShotgun = aIP.ammoShotgun - 1;//subtract one bullet from gun

            shotgunColliderCenPos = shotgunRangeCollider.bounds.center; //need to obtain the center position of the shotgun box collider.
            shotgunColliderRotation = shotgunRangeCollider.transform.rotation; //need to obtain the rotation of the shotgun box collider.
            angleInDegrees = shotgunColliderRotation.eulerAngles.z;
            enemiesInRangeArr = Physics2D.OverlapBoxAll(shotgunColliderCenPos, shotgunCollidersize, angleInDegrees);

            foreach (Collider2D collider in enemiesInRangeArr) //this is working a little better now.
            {
                Debug.Log("Enemies within the shotgun box collider when FIRED are " + collider.gameObject.name);//collider.gameObject.name);
            }
            enemiesInRangeArr = new Collider2D[0]; //empty out the array for reuse.

        }
        else if (Input.GetKeyDown(KeyCode.K) && aIP.ammoShotgun <= 0 && aPM.holdShotgun)
        {
            aPM.holdShotgun = false;
            anime.SetBool("HoldShotgun", aPM.holdShotgun);
            SHOTGUN.SetActive(false);
            Debug.Log("No ammo!");
        }
        //checks for a left mouse button input along with other restrictions if needed
        if (Input.GetMouseButton(0) && coolDownTimer > attackCoolDown && playerMovement.canAttack()) {
            Attack();
        }
        coolDownTimer += Time.deltaTime;
    }
    //handles the Attack input
    private void Attack(){
        //starts the attack animation
        anime.SetTrigger("Attack");
        //checks if we hit an enemy
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        //resets our attack cooldown
        coolDownTimer = 0;
        //sends a console update for each enemy we hit
        foreach(Collider2D enemy in hitEnemies){ //for each ...., add it to the array/list
            Debug.Log("We hit " + enemy.name);
        }
    }
    //helps visualize the attack point in unity
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(shotgunColliderCenPos, shotgunCollidersize/4); //this method takes in Vector3 parameters so not sure if it is a correct depiction. Dividing by 4 makes it accurately shaped.
    }
}
