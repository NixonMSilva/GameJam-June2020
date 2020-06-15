using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    bool isSprinting;

    public Rigidbody2D rb;

    public MovementController mc;

    Vector2 movement;

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
}
