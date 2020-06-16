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
        // Debug.Log("Angle: " + playerRotation);
        // Debug.Log("Sin: " + Mathf.Sin(playerRotation));
        // Debug.Log("Cos: " + Mathf.Cos(playerRotation));
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

    }    

    IEnumerator CooldownSmoke (float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        // Debug.Log("Whelp");
        isThrowingSmoke = false;
    }
}
