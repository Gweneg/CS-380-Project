using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Il2Cpp;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCoolDown;
    private Animator anime;
    private PlayerMovement playerMovement;
    private float coolDownTimer = Mathf.Infinity;
    public Transform attackPoint;
    public float attackRange = .5f;
    public LayerMask enemyLayers;
    // Awake is called before the first frame update
    private void Awake()
    {
        anime = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }
    // Update registers user input
    private void Update()
    {
        //checks for a left mouse button input along with other restrictions if needed
        if(Input.GetMouseButton(0) && coolDownTimer > attackCoolDown && playerMovement.canAttack()){
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
