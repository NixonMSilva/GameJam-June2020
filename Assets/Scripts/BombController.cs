using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    public float bombSpeed;
    public float flightRange;

    public float solidnessTimeout;

    public Vector2 playerDirection;

    Vector3 originalPos;
    Vector3 currentPos;

    public Rigidbody2D rb;
    public MovementController mc;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        StartCoroutine(BecomeSolid(solidnessTimeout));
    }

    // Update is called once per frame
    void Update()
    {
        currentPos = transform.position;
        if (GetDistanceFromOrigin(currentPos) <= flightRange)
        {
            BombFly();
        }
        else
        {
            Explosion();
        }
        
    }

    public void BombFly ()
    {
        mc.PerformMoveNormalized(rb, playerDirection, bombSpeed);
    }

    void Explosion ()
    {
        //  Debug.Log("Exuprosion!");
        rb.isKinematic = true;
    }
    
    float GetDistanceFromOrigin (Vector3 currentPosition)
    {
        return Vector3.Distance(currentPosition, originalPos);
    }

    IEnumerator BecomeSolid (float timeout)
    {
        yield return new WaitForSeconds(timeout);
        GetComponent<CircleCollider2D>().isTrigger = false;
    }
}
