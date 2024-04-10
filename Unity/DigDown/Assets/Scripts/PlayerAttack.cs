using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Il2Cpp;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public ItemPickup aIP; //-
    public PlayerMovement aPM; //--//-- important, keep watch if this works well

    [SerializeField] private float attackCoolDown;
    private Animator anime;

    public GameObject SHOTGUN;
    public Animator shotgunAnimator; //-
    //public Animation shotgunAnimation; //-
    private PlayerMovement playerMovement;
    private float coolDownTimer = Mathf.Infinity;
    public Transform attackPoint;
    public float attackRange = .5f;
    public LayerMask enemyLayers;
    
    // Awake is called before the first frame update
    private void Awake()
    {
        //gained access to the SHOTGUN gameobject, and from there I had access to its components, hence shotgunAnimator = SHOTGUN.GetComponent<Animator>();
        SHOTGUN = GameObject.Find("Shotgun");
        //shotgunAnimation = SHOTGUN.GetComponent<Animation>();//-
        shotgunAnimator = SHOTGUN.GetComponent<Animator>();
        anime = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        //shotgunAnimation.
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
        if (Input.GetKeyDown(KeyCode.K) && aIP.ammoShotgun > 0) //Recall that keyDown works more like a keyPress.
        {
            aIP.ammoShotgun = aIP.ammoShotgun - 1;
            shotgunAnimator.Play("Shotgun Firing", 0, 0f); //ensures that whenever condition is met, immediately the animation restarts from the beginning.
            //shotgunAnimator.SetTrigger("ShootShotgunTrig"); //replacing this with a trigger may be a better option than using a bool.
            Debug.Log("aIP.ammoShotgun value is " + aIP.ammoShotgun); //one issue is it kept saying the value was 0.
            //I believe it was because it kept picking the ammoShotgun global variable which was initialized to 0. I believe it just kept
            //picking up the variable as it was set to 0. ANSWER: the script was not placed within the public variable aIP in the inspector.
            Debug.Log("Shotgun has been fired");

            //Once the ammo reaches 0 and the shooting animation has finished, holdShotgun parameter can now be set to false, shotgun object set to false, etc
                    //if (!(shotgunAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shotgun Firing")) && (aIP.ammoShotgun == 0))
                    //{
                    //    aPM.holdShotgun = false; //or try aIP.aPM.holdShotgun = false;
                    //    Debug.Log("shooting animation has finished and the ammo is depleted.");
                    //    aIP.anime.SetBool("HoldShotgun", aPM.holdShotgun); //or try aIP.aPM.holdShotgun
                    //    SHOTGUN.SetActive(false);
                    //    Debug.Log("No ammo");
                    //}
        } else if (Input.GetKeyDown(KeyCode.K) && aIP.ammoShotgun <= 0) //try shooting but clip is already empty
        {
            aPM.holdShotgun = false;
            aIP.anime.SetBool("HoldShotgun", aPM.holdShotgun);
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
