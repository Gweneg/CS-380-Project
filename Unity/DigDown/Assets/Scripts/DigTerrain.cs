using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Il2Cpp;
using UnityEngine;

public class DigTerrain : MonoBehaviour
{
    [SerializeField] private float digCoolDown;
    //private Animator anime;
    private PlayerMovement playerMovement;
    private float coolDownTimer = Mathf.Infinity;
    public LayerMask terrainLayers;
    public Transform digPoint;
    public float digRange = .1f;
    private Vector3 mousePos;
    private Camera mainCam;
    private GameObject block;
    // Start is called before the first frame update
    void Awake()
    {
        //anime = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
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
        if(Input.GetMouseButton(1) && coolDownTimer > digCoolDown){
            Dig();
        }
        coolDownTimer += Time.deltaTime;
    }
    private void Dig(){
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
