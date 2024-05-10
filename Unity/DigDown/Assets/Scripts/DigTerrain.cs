using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Il2Cpp;
using UnityEngine;

public class DigTerrain : MonoBehaviour
{
    [SerializeField] private float digCoolDown;
    public Animator anime; //was commented out //was private
    private PlayerMovement playerMovement;
    private float coolDownTimer = Mathf.Infinity;
    public LayerMask terrainLayers;
    public Transform digPoint;
    public float digRange = .1f;
    private Vector3 mousePos;
    private Camera mainCam;
    private GameObject block;

    public GameObject PLAYER;
    public bool isDigging = false;
    public AnimatorStateInfo DigState;
    public ItemPickup aIP;

    // Start is called before the first frame update
    public void Awake() //was not private or public (blank)
    {
        PLAYER = GameObject.Find("New Dwarf");
        anime = PLAYER.GetComponent<Animator>(); //was commented out
        playerMovement = GetComponent<PlayerMovement>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    public void Update() //was blank
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePos - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        if (transform.parent.localScale.x < 0){
            transform.rotation = Quaternion.Euler(180, 180, rotZ);
        }
        if (transform.parent.localScale.x > 0){
            transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }

        if(Input.GetMouseButton(1) && coolDownTimer > digCoolDown && aIP.holdPickaxe == true){ //original code //if(Input.GetMouseButton(1) && coolDownTimer > digCoolDown)
            anime.SetTrigger("DigTrig");
            Dig();
        }
        coolDownTimer += Time.deltaTime;
    }
    public void Dig(){ //was private
        Collider2D[] dugTerrain = Physics2D.OverlapCircleAll(digPoint.position, digRange, terrainLayers);
        coolDownTimer = 0;
        foreach(Collider2D terrain in dugTerrain){
            Debug.Log("We dug " + terrain.name);
            block = GameObject.Find(terrain.name);
            Destroy(block);
        }
    }
    void OnDrawGizmosSelected()
    {
        if (digPoint == null){
            return;
        }
        Gizmos.DrawWireSphere(digPoint.position, digRange);
    }
}
