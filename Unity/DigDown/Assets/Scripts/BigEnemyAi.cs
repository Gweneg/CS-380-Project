using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;

public class BigEnemyAi : MonoBehaviour
{

    public float HP = 9;

    //public bool isJumping;
    void Start()
    {

    }
    //flip sprite
    private void Awake()
    {

    }
    void Update()
    {
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

}