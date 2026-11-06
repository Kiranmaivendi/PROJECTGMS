using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class RunnerPlayer : MonoBehaviour
{
    [Header("Running")]
    public float forwardSpeed = 10f;
    public float maxSpeed = 25f;
    public float speedIncreasePerSecond = 0.2f;

    [Header("Lanes")]
    public float laneDistance = 1.6f;     // Distance between lanes
    public float laneChangeSpeed = 12f;   // How fast the player moves sideways

    [Header("Jump and Slide")]
    public float jumpForce = 7f;
    public float slideDuration = 0.8f;

    [HideInInspector] public bool isAlive = true;

    private Rigidbody rb;
    private CapsuleCollider col;
    private int currentLane = 1;          // 0 = left, 1 = middle, 2 = right
    private bool wantJump;
    private bool wantSlide;
    private bool isSliding;
    private float slideTimer;
    private float originalHeight;
    private Vector3 originalCenter;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        originalHeight = col.height;
        originalCenter = col.center;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (!isAlive) return;

        // Speed up slowly over time
        forwardSpeed = Mathf.Min(maxSpeed, forwardSpeed + speedIncreasePerSecond * Time.deltaTime);

        ReadInput();

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f) StopSlide();
        }
    }

    void ReadInput()
    {
        bool left = false, right = false, jump = false, slide = false;

#if ENABLE_INPUT_SYSTEM
        Keyboard k = Keyboard.current;
        if (k != null)
        {
            left  = k.aKey.wasPressedThisFrame || k.leftArrowKey.wasPressedThisFrame;
            right = k.dKey.wasPressedThisFrame || k.rightArrowKey.wasPressedThisFrame;
            jump  = k.spaceKey.wasPressedThisFrame || k.wKey.wasPressedThisFrame || k.upArrowKey.wasPressedThisFrame;
            slide = k.sKey.wasPressedThisFrame || k.downArrowKey.wasPressedThisFrame;
        }
#else
        left  = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        right = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        jump  = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        slide = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
#endif

        if (left)  currentLane = Mathf.Max(0, currentLane - 1);
        if (right) currentLane = Mathf.Min(2, currentLane + 1);
        if (jump)  wantJump = true;
        if (slide) wantSlide = true;
    }

    void FixedUpdate()
    {
        if (!isAlive)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        float targetX = (currentLane - 1) * laneDistance;
        float xVelocity = (targetX - rb.position.x) * laneChangeSpeed;

        Vector3 v = rb.linearVelocity;
        v.x = xVelocity;
        v.z = forwardSpeed;

        bool grounded = IsGrounded();

        if (wantJump && grounded)
        {
            if (isSliding) StopSlide();
            v.y = jumpForce;
        }

        if (wantSlide && grounded && !isSliding)
        {
            StartSlide();
        }

        wantJump = false;
        wantSlide = false;
        rb.linearVelocity = v;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(col.bounds.center, Vector3.down, col.bounds.extents.y + 0.15f);
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;
        col.height = originalHeight * 0.5f;
        col.center = new Vector3(originalCenter.x, originalCenter.y - originalHeight * 0.25f, originalCenter.z);
        transform.localScale = new Vector3(1f, 0.5f, 1f);
    }

    void StopSlide()
    {
        isSliding = false;
        col.height = originalHeight;
        col.center = originalCenter;
        transform.localScale = Vector3.one;
    }

    // Called later by obstacles and the GameManager
           public void Die()
    {
        if (!isAlive) return;
        isAlive = false;
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayCrash();
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}