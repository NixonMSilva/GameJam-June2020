using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    bool isSprinting;
    bool isThrowingSmoke;
    bool isSwingingCane;

    public float smokeCooldownLimit;
    public float caneCooldownLimit;

    public GameObject smokeBomb;
    GameObject currentSmokeBomb;

    public Rigidbody2D rb;

    public MovementController mc;

    public PolygonCollider2D caneCollider;

    Vector2 movement;

    ContactFilter2D guardFilter;

    public LayerMask npcMask;
    public LayerMask guardMask;

    // Start is called before the first frame update
    void Awake ()
    {
        isThrowingSmoke = false;
        isSwingingCane = false;
        caneCollider = GetComponentInChildren<PolygonCollider2D>();
        guardFilter.layerMask = guardMask;
        guardFilter.useLayerMask = true;
    }

    // Update is called once per frame
    void Update ()
    {
        // Input handling
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        /* Check if player hasn't thrown smoke in the last X seconds (cooldown time), if he didn't, 
        then he can throw a new one by pressing the button */
        if (!isThrowingSmoke && Input.GetKeyDown(KeyCode.G))
        {
            ThrowSmoke();
            StartCoroutine(CooldownSmoke(smokeCooldownLimit));
        }
        else if (!isThrowingSmoke && Input.GetKeyDown(KeyCode.H))
        {
            PlantSmoke();
            StartCoroutine(CooldownSmoke(smokeCooldownLimit));
        }

        /* Check if the player hasn't swung the cane in the last X seconds (cooldown time), if he didn't
        then he can swing the cane again by pressing the button */
        if (!isSwingingCane && Input.GetKeyDown(KeyCode.F))
        {
            SwingCane();
            StartCoroutine(CooldownCane(caneCooldownLimit));
        }

        /* If the player presses the key to interact with an NPC, check if that is possible through
        the respective function */
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InteractWithNPC();
        }
    }

    void FixedUpdate ()
    {
        // Movement handling
        if (isSprinting)
            speed = 4.5f;
        else
            speed = 2.5f;
        mc.PerformMove(rb, movement, speed);
    }

    void ThrowSmoke ()
    {
        float playerRotation = rb.gameObject.transform.rotation.eulerAngles.z;
        Vector2 direction = new Vector2 (Mathf.Cos(playerRotation * Mathf.Deg2Rad), Mathf.Sin(playerRotation * Mathf.Deg2Rad));
        currentSmokeBomb = Instantiate(smokeBomb, transform.position, transform.rotation);
        currentSmokeBomb.GetComponent<BombController>().playerDirection = direction;
        isThrowingSmoke = true;
    }

    void PlantSmoke ()
    {
        currentSmokeBomb = Instantiate(smokeBomb, transform.position, transform.rotation);
        currentSmokeBomb.GetComponent<BombController>().isThrown = false;
        isThrowingSmoke = true;
    }

    void SwingCane ()
    {
        Collider2D[] hits = new Collider2D [2];
        //  ContactFilter2D cf = new ContactFilter2D();
        isSwingingCane = true;
        if (caneCollider.IsTouchingLayers(guardMask))
        {
            if (caneCollider.OverlapCollider(guardFilter, hits) > 0)
            {
                for (int i = 0; i < hits.Length; ++i)
                {
                    if (hits[i] != null)
                        hits[i].GetComponent<GuardController>().GetStunned();
                }
                
            }
        }
    }

    void InteractWithNPC ()
    {
        RaycastHit2D rc = Physics2D.CircleCast((Vector2)transform.position, 2f, Vector2.zero, 2f, npcMask);
        if (rc)
        {
            NPCController npcc = rc.collider.gameObject.GetComponent<NPCController>();
            if (npcc.IsInteractable())
            {
                npcc.CalmDown();
            }
        }
        
    }    

    IEnumerator CooldownSmoke (float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        isThrowingSmoke = false;
    }

    IEnumerator CooldownCane (float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        isSwingingCane = false;
        Debug.Log("Can swing again!");
    }
}
