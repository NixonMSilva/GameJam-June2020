﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : NPCController
{
    public float minChaseRange;
    public float maxChaseRange;
    public float chaseSpeed;
    public float dodgeSpeed;

    public GameObject player;

    PlayerController pc;

    Vector2 guardOrigin;
    Quaternion guardRotationOrigin;

    float distanceToPlayer;

    bool isChasingPlayer;
    bool isReturningToOrigin;

    public bool isVulnerable;
    public bool isStunned;

    public LayerMask wallMask;

    void Awake ()
    {
        guardOrigin = this.transform.position;
        guardRotationOrigin = this.transform.rotation;
        distanceToPlayer = 100f;
        movementIndex = 0;
        isReturningToOrigin = false;
        isChasingPlayer = false;
        isStunned = false;
        pc = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update ()
    {
        
    }

    void FixedUpdate ()
    {
        // Don't do anything if the guard is stunned
        if (!isStunned)
        {
            // Updates the distance between the guard and the player
            UpdateDistanceToPlayer();

            // Chase the player if he gets too close (minChaseRange), and then stops chasing while he doesn't leave his range (maxChaseRange)
            if (((!isChasingPlayer) && (distanceToPlayer <= minChaseRange)) || (isChasingPlayer) && (distanceToPlayer <= maxChaseRange))
            {
                // If the player isn't behind a wall and the guard hasn't started chasing the player yet
                if (!ChaseOccluder() || isChasingPlayer)
                {
                    isChasingPlayer = true;
                    isReturningToOrigin = false;

                    // Stop chasing if the player already got caught
                    if (!pc.GotCaught())
                        ChasePlayer();
                }
            }
            else if ((isChasingPlayer) && (distanceToPlayer >= maxChaseRange))
            {
                // Debug.Log("Stopped chasing player!");
                StopChasingPlayer();
            }

            // If the guard isn't chasing the player (got out of sight or range) and he isn't returning to origin, then prompt him to go back to his origin
            if ((!isChasingPlayer) && (!isReturningToOrigin) && (Vector3.Distance(rb.position, (Vector3)guardOrigin) >= 0.25f))
            {
                isReturningToOrigin = true;
            }

            // If the guard isn't chasing the player but it's out of its position then he returns to it
            if (isReturningToOrigin)
            {
                if (Vector3.Distance(rb.position, (Vector3)guardOrigin) <= 0.25f)
                {
                    isReturningToOrigin = false;
                }
                ReturnToOrigin();
            }
        }
    }

    bool ChaseOccluder ()
    {
        Debug.DrawRay(this.transform.position, player.transform.position - this.transform.position);
        RaycastHit2D rc = Physics2D.Raycast((Vector2) this.transform.position, (Vector2) (player.transform.position - this.transform.position), distanceToPlayer, wallMask);
        if (rc)
        {
            Debug.Log("Occluded!");
            return true;
        }
        return false;
    }

    void ChasePlayer ()
    {
        movement = player.transform.position - this.transform.position;
        mc.PerformMoveNormalized(rb, movement, chaseSpeed);
    }

    void UpdateDistanceToPlayer ()
    {
        distanceToPlayer = Vector2.Distance(rb.position, player.GetComponent<Rigidbody2D>().position);
    }

    void StopChasingPlayer () { isChasingPlayer = false; }

    void ReturnToOrigin ()
    {
        movement = (guardOrigin - ((Vector2) this.transform.position));
        mc.PerformMoveNormalized(rb, movement, chaseSpeed);
        // If the guard is back at his position then he should go back to its original rotation
        if (Vector3.Distance(rb.position, (Vector3) guardOrigin) <= 0.25f)
        {
            this.transform.rotation = guardRotationOrigin;
        }
    }

    public void GetStunned ()
    {
        if (isVulnerable)
        {
            isStunned = true;
            Debug.Log("Yer stunned");
            isVulnerable = false;
            StartCoroutine(VulnerableCooldown());
        }
    }

    IEnumerator VulnerableCooldown ()
    {
        yield return new WaitForSeconds(5f);
        isStunned = false;
        anim.SetBool("isStunned", false);
    }
}
