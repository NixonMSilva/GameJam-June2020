using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public int currentOrientation;

    // Start is called before the first frame update
    void Start ()
    {
        
    }

    // Update is called once per frame
    void Update ()
    {
        
    }

    public void PerformMove (Rigidbody2D rb, Vector2 movementVector, float speed)
    {
        AdjustOrientation(movementVector);
        rb.MovePosition(rb.position + movementVector * speed * Time.fixedDeltaTime);
        
    }

    public void PerformMoveNormalized (Rigidbody2D rb, Vector2 movementVector, float speed)
    {
        movementVector.Normalize();
        PerformMove(rb, movementVector, speed);
    }


    public void AdjustOrientation (Vector2 movementVector)
    {
        Vector2 movementDirection = movementVector;
        //Debug.Log(movementVector);
        if (movementDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
            this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
