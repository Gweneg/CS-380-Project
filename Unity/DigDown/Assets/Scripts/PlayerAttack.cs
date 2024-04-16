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
    public DetectShots aDS;

    [SerializeField] private float attackCoolDown;
    private Animator anime;

    public GameObject ENEMY;

    public GameObject SHOTGUN; //game object variable
    public Animator shotgunAnimator; //component variable

    public GameObject SHOTGUN_AUDIO_SOURCE; //**
    public AudioSource shotgunAudioSource;//component variable
    public AudioClip explosionAudioClip;//component varibale - assign the audio clip you want to play to the explosionAudioClip field in the Inspector

    private PlayerMovement playerMovement; //-reference variable to the ItemPickup class
    private float coolDownTimer = Mathf.Infinity;
    public Transform attackPoint;
    public float attackRange = .5f;
    public LayerMask enemyLayers;
    
    // Awake is called before the first frame update
    private void Awake()
    {
        ENEMY = GameObject.Find("enemy1");

        //gained access to the SHOTGUN gameobject, and from there I had access to its components, hence shotgunAnimator = SHOTGUN.GetComponent<Animator>();
        SHOTGUN = GameObject.Find("Shotgun");
        //shotgunAnimation = SHOTGUN.GetComponent<Animation>();//-
        shotgunAnimator = SHOTGUN.GetComponent<Animator>();
        anime = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        SHOTGUN_AUDIO_SOURCE = GameObject.Find("shotgunAudioObject"); //**
        shotgunAudioSource = SHOTGUN_AUDIO_SOURCE.GetComponent<AudioSource>();//**
        //shotgunAudioSource = SHOTGUN.GetComponent<AudioSource>();//--//--
        shotgunAudioSource.clip = explosionAudioClip;//
    }
    // Update registers user input
    public void Update() //- was private
    {
        //checks for a left mouse button input along with other restrictions if needed
        if (Input.GetMouseButton(0) && coolDownTimer > attackCoolDown && playerMovement.canAttack()) {
            Attack();
        }
        coolDownTimer += Time.deltaTime;
        //**Everytime this condition is met v, the shoot animation should restart. SUCCESS
        //**There should also be a coolDown attack timer, but not too high. NVM
        if (Input.GetKeyDown(KeyCode.K) && aIP.ammoShotgun > 0 && aPM.holdShotgun) //Recall that keyDown works more like a keyPress.
        {
            if (aDS.inRange == true)
            {
                ENEMY.SetActive(false);
            }
            aIP.ammoShotgun = aIP.ammoShotgun - 1;
            shotgunAnimator.Play("Shotgun Firing", 0, 0f); //ensures that whenever condition is met, immediately the animation restarts from the beginning.
            //-Code for "shooting shotgun sound" goes here.
            shotgunAudioSource.Play(); //CURRENT ISSUE: If I spam shoot the gun too fast, on the final shot attempted without ammo would
            //deactivate the shotgun game object, therefore also deactivating the audio clip attached to it. So in this spam scenario, the
            //audio would just get cut out on the final shot. (Potentially just attach the audio source component to another game Object)

            //-Projectile/raycast code goes here
            //-Must add projectile(visible or not) to hurt enemies

            Debug.Log("aIP.ammoShotgun value is " + aIP.ammoShotgun); //one issue is it kept saying the value was 0.
            //I believe it was because it kept picking the ammoShotgun global variable which was initialized to 0. I believe it just kept
            //picking up the variable as it was set to 0. ANSWER: the script was not placed within the public variable aIP in the inspector.
            Debug.Log("Shotgun has been fired");

        } else if (Input.GetKeyDown(KeyCode.K) && aIP.ammoShotgun <= 0 && aPM.holdShotgun) //try shooting but clip is already empty
        {
            aPM.holdShotgun = false;
            anime.SetBool("HoldShotgun", aPM.holdShotgun);//aIP.anime.SetBool("HoldShotgun", aPM.holdShotgun);
            SHOTGUN.SetActive(false);
            Debug.Log("No ammo");
        }
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
        foreach(Collider2D enemy in hitEnemies){
            Debug.Log("We hit " + enemy.name);
        }
    }
    //helps visualize the attack point in unity
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null){
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
