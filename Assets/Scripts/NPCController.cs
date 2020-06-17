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

    public bool isInteractable;
    public bool isCalmed;

    public Animator anim;

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
            
    }

    public void MakeInteractable ()
    {
        isInteractable = true;
        anim.SetBool("isInteractable", true);
        // Start cooldown to turn the NPC normal again
        StartCoroutine(InteractableCooldown());
    }

    public void CalmDown ()
    {
        isCalmed = true;
        anim.SetBool("isCalmed", true);
    }

    public bool IsInteractable () { return isInteractable; }
    public bool IsCalmed () { return isCalmed; }

    IEnumerator InteractableCooldown ()
    {
        yield return new WaitForSeconds(5f);
        isInteractable = false;
        anim.SetBool("isInteractable", false);
    }
}
