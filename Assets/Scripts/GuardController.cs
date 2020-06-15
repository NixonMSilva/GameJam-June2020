using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : NPCController
{
    public float minChaseRange;
    public float maxChaseRange;
    public float chaseSpeed;

    public GameObject player;

    Vector2 guardOrigin;
    Quaternion guardRotationOrigin;

    float distanceToPlayer;

    bool isChasingPlayer;
    bool isReturningToOrigin;

    void Awake ()
    {
        guardOrigin = this.transform.position;
        guardRotationOrigin = this.transform.rotation;
        distanceToPlayer = 100f;
        movementIndex = 0;
        isReturningToOrigin = false;
        isChasingPlayer = false;
    }

    // Update is called once per frame
    void Update ()
    {
        
    }

    void FixedUpdate ()
    {
        // Updates the distance between the guard and the player
        UpdateDistanceToPlayer();

        // Chase the player if he gets too close (minChaseRange), and then stops chasing while he doesn't leave his range (maxChaseRange)
        if (((!isChasingPlayer) && (distanceToPlayer <= minChaseRange)) || (isChasingPlayer) && (distanceToPlayer <= maxChaseRange))
        {
            isChasingPlayer = true;
            isReturningToOrigin = false;
            ChasePlayer();
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
            ReturnToOrigin ();
        }
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
  

}
