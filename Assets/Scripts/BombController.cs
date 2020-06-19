using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class BombController : MonoBehaviour
{
    public float bombSpeed;
    public float flightRange;

    public float solidnessTimeout;

    public int playerDirection;

    Vector3 originalPos;
    Vector3 currentPos;

    public Rigidbody2D rb;
    public MovementController mc;

    public Animator anim;

    bool exploded = false;
    bool fellDown = false;

    public bool isThrown;

    public LayerMask npcMask;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        isThrown = true;
        StartCoroutine(BecomeSolid(solidnessTimeout));
        StartCoroutine(FallDown());
    }

    // Update is called once per frame
    void Update()
    {
        currentPos = transform.position;
        if ((GetDistanceFromOrigin(currentPos) <= flightRange) && (!fellDown) && (isThrown))
        {
            BombFly();
        }
        else
        {
            if (!exploded)
            {
                // Debug.Log("Explodes!");
                Explosion();
            }
        }
    }

    public void BombFly ()
    {
        mc.PerformMoveAnimationless(rb, playerDirection, bombSpeed);
    }

    void Explosion ()
    {
        //  Debug.Log("Exuprosion!");
        exploded = true;
        rb.isKinematic = true;
        anim.SetBool("isExploded", true);
        RaycastHit2D[] rc;
        rc = Physics2D.CircleCastAll((Vector2)transform.position, 2f, Vector2.zero, 2f, npcMask);
        foreach (RaycastHit2D cast in rc)
        {
            // Debug.Log(cast.collider.gameObject.name);
            cast.collider.gameObject.GetComponent<NPCController>().MakeInteractable();
        }
        
        // NPCController npcc = rc.collider.gameObject.GetComponent<NPCController>();
        // npcc.MakeInteractable();
        Destroy(this.gameObject);
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

    IEnumerator FallDown ()
    {
        yield return new WaitForSeconds(1.6f);
        fellDown = true;
    }
}
