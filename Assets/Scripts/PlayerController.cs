using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    bool isSprinting;
    bool isThrowingSmoke;

    public int smokeCooldownLimit;

    public GameObject smokeBomb;
    GameObject currentSmokeBomb;

    public Rigidbody2D rb;

    public MovementController mc;

    Vector2 movement;

    IEnumerator bombCooldown;

    public LayerMask npcMask;

    // Start is called before the first frame update
    void Awake ()
    {
        speed = 2f;
    }

    // Update is called once per frame
    void Update ()
    {
        // Input handling
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        /* Check if player hasn't thrown smoke in the last X frames (cooldown time), if he didn't, 
        then he can throw a new one by pressing the button */
        if (!isThrowingSmoke && Input.GetKeyDown(KeyCode.G))
        {
            ThrowSmoke();
            StartCoroutine(CooldownSmoke(smokeCooldownLimit));
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

    void SwingCane ()
    {

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
}
