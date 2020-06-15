using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public float speed;

    public int movementIndex;

    public Rigidbody2D rb;

    public MovementController mc;

    public Vector2 movement;

    // Start is called before the first frame update
    void Awake ()
    {
        movementIndex = 0;
        speed = 2f;
    }

    // Update is called once per frame
    void Update ()
    {
        if (movementIndex >= Int32.MaxValue)
            movementIndex = 0;
    }

    void FixedUpdate ()
    {
        MovementDecision ();    
    }

    void MovementDecision ()
    {
        Vector2 mv = new Vector2 (1, 0);
        Vector2 mv2 = new Vector2 (1, 1);
        if (movementIndex++ % 5 == 0) 
        {
            mc.PerformMoveNormalized(rb, mv, speed);
        }
        else
        {
            mc.PerformMoveNormalized(rb, mv2, speed);
        }
            
    }
}
