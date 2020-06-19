﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public int currentOrientation;
    public Animator anim;

    Transform cane;

    bool isPlayer = false;

    // Start is called before the first frame update
    void Start ()
    {
        if (this.gameObject.name == "Player")
        {
            isPlayer = true;
            cane = gameObject.transform.Find("CaneRange");
        }
    }

    // Update is called once per frame
    void Update ()
    {
        
    }

    public void PerformMove (Rigidbody2D rb, Vector2 movementVector, float speed)
    {
        //AdjustOrientation(movementVector);
        AdjustSpriteOrientation(movementVector);
        rb.MovePosition(rb.position + movementVector * speed * Time.fixedDeltaTime);
        
    }

    public void PerformMoveNormalized (Rigidbody2D rb, Vector2 movementVector, float speed)
    {
        movementVector.Normalize();
        PerformMove(rb, movementVector, speed);
    }

    public void PerformMoveAnimationless (Rigidbody2D rb, int playerOrientation, float speed)
    {
        Vector2 movementVector;
        if (playerOrientation == 0)
        {
            movementVector.x = 0f;
            movementVector.y = -1f;
        }
        else if (playerOrientation == 1)
        {
            movementVector.x = -1f;
            movementVector.y = 0f;
        }
        else if (playerOrientation == 2)
        {
            movementVector.x = 0f;
            movementVector.y = 1f;
        }
        else if (playerOrientation == 3)
        {
            movementVector.x = 1f;
            movementVector.y = 0f;
        }
        else
        {
            movementVector = Vector2.zero;
        }
        
        rb.MovePosition(rb.position + movementVector * speed * Time.fixedDeltaTime);
    }

    public void AdjustSpriteOrientation (Vector2 movementVector)
    {
        if (movementVector.x != 0 || movementVector.y != 0)
        {
            if (movementVector.y > 0)
                currentOrientation = 2; 
            else if (movementVector.y < 0)
                currentOrientation = 0;

            if (movementVector.x > 0)
                currentOrientation = 3;
            else if (movementVector.x < 0)
                currentOrientation = 1;
            anim.SetFloat("horizontal", movementVector.x);
            anim.SetFloat("vertical", movementVector.y);
            // If it's a player movement, change the cane gizmo
            if (isPlayer)
                AdjustOrientation(movementVector, cane);  
        }
        anim.SetFloat("speed", movementVector.magnitude);
        // Debug.Log(this.gameObject.name + "'s speed: " + movementVector.magnitude);
    }


    public void AdjustOrientation (Vector2 movementVector, Transform tr)
    {
        Vector2 movementDirection = movementVector;
        //Debug.Log(movementVector);
        if (movementDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
            tr.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        }
    }
}
