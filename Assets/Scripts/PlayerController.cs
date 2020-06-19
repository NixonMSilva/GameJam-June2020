using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed;

    bool isSprinting;
    bool isThrowingSmoke;
    bool isSwingingCane;

    bool isCaught;
    bool canUpdatePlayer;
    bool victoryController;

    bool isTooltipShowable;

    int calmedCount;

    public float smokeCooldownLimit;
    public float caneCooldownLimit;

    public GameObject gameOverOverlay;
    public GameObject uiOverlay;
    public GameObject tooltipOverlay;

    public GameObject smokeBomb;
    GameObject currentSmokeBomb;

    public Rigidbody2D rb;

    public MovementController mc;

    public BoxCollider2D bc;

    public PolygonCollider2D caneCollider;

    public GameObject npcCounter;

    Vector2 movement;

    ContactFilter2D guardFilter;

    public LayerMask npcMask;
    public LayerMask guardMask;

    // Start is called before the first frame update
    void Awake ()
    {
        calmedCount = 0;
        // Update the number of calmable NPCs
        GameObject[] smokable = GameObject.FindGameObjectsWithTag("Smokable");
        foreach (GameObject npc in smokable)
        {
            if (!npc.GetComponent<NPCController>().IsCalmed())
            {
                calmedCount++;
                Debug.Log("Calmable NPC found: " + npc.name);
            }
        }
        npcCounter.GetComponent<CounterNPC>().SetValue(calmedCount);

        // Initialize variables
        isTooltipShowable = false;
        isThrowingSmoke = false;
        isSwingingCane = false;
        isCaught = false;
        victoryController = false;
        canUpdatePlayer = true;
        caneCollider = GetComponentInChildren<PolygonCollider2D>();
        guardFilter.layerMask = guardMask;
        guardFilter.useLayerMask = true;
    }

    // Update is called once per frame
    void Update ()
    {
        if (calmedCount <= 0)
        {
            WinGame();
        }

        // Check if you can show the tooltip
        tooltipOverlay.SetActive(CanShowTooltip());

        // Freezes the player if he get caughts by a guard
        if (!isCaught)
        {
            // Input handling
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            isSprinting = Input.GetKey(KeyCode.LeftShift);

            // Check if the player isn't being aprehended (touched) by a guard
            isCaught = IsBeingAprehended();

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
        else if (canUpdatePlayer)
        {
            BeAprehended();
        }
    }

    void FixedUpdate ()
    {
        // Movement handling
        if (isSprinting)
            speed = 4.5f;
        else
            speed = 2.5f;
        if (!isCaught)
            mc.PerformMove(rb, movement, speed);
    }

    bool CanShowTooltip ()
    {
        RaycastHit2D rc = Physics2D.CircleCast((Vector2)transform.position, 2f, Vector2.zero, 2f, npcMask);
        if (rc)
        {
            NPCController npcc = rc.collider.gameObject.GetComponent<NPCController>();
            if (!npcc.isCalmed && npcc.isInteractable)
                return true;
        }
        return false;            
    }

    void ThrowSmoke ()
    {
        float playerRotation = rb.gameObject.transform.rotation.eulerAngles.z;
        Vector2 direction = new Vector2 (Mathf.Cos(playerRotation * Mathf.Deg2Rad), Mathf.Sin(playerRotation * Mathf.Deg2Rad));
        currentSmokeBomb = Instantiate(smokeBomb, transform.position, transform.rotation);
        currentSmokeBomb.GetComponent<BombController>().playerDirection = this.GetComponent<MovementController>().currentOrientation;
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
                if (!npcc.IsCalmed())
                {
                    isTooltipShowable = false;
                    npcc.CalmDown();
                    calmedCount--;
                    npcCounter.GetComponent<CounterNPC>().SetValue(calmedCount);
                }
            }
        }
    }

    bool IsBeingAprehended ()
    {
        // Do not aprehend player if victory is fulfilled
        if ((bc.IsTouchingLayers(guardMask)) && (!victoryController))
        {
            return true;
        }
        return false;
    }
    
    void BeAprehended ()
    {
        Debug.Log("Game Over!");
        canUpdatePlayer = false;
        gameOverOverlay.SetActive(true);
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

    public void RestartGame ()
    {
        SceneManager.LoadScene("Controls");
    }

    public bool GotCaught ()
    {
        return isCaught;
    }

    void SpawnNewGuard ()
    {

    }

    void WinGame ()
    {
        victoryController = true;
        Debug.Log("Victory!");
        SceneManager.LoadScene("Victory");
    }
}
