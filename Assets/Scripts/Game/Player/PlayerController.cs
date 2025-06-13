using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // ==================================================
    //                  SERIALIZED FIELDS
    //  (Variables that you can see and edit in the Unity Inspector)
    // ==================================================

    [Header("Configuration")]
    [SerializeField] private readonly float movementSpeed = 5f;
    [SerializeField] private readonly float sleepDuration = 5f;
    [SerializeField] private readonly float eatDuration = 2f;
    [SerializeField] private readonly float maxStatValue = 100f;
    [SerializeField] private readonly float hungerDecayRate = 2f;    // Hunger points lost per second
    [SerializeField] private readonly float sleepyDecayRate = 1.0f;  // Sleepiness points lost per second

    [Header("Component References")]
    [SerializeField] private Joystick joystick;
    [SerializeField] private Animator animator;

    // ==================================================
    //                  PUBLIC STATE
    //  (Variables that other scripts, like UIManager, might need to read)
    // ==================================================
    
    public float hungerStatValue = 100f;
    public float sleepyStatValue = 100f;

    // ==================================================
    //                  PRIVATE STATE & CACHED COMPONENTS
    //  (Internal variables for this script's logic)
    // ==================================================

    private SpriteRenderer spriteRenderer;
    private Vector2 movementInput;
    private Rigidbody2D rb;
    private PlayerControls playerControls;

    private bool isSleeping;
    private bool isEating;

    // ==================================================
    //                      SETUPS
    // ==================================================

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        InitializeInputSystem();
    }

    private void InitializeInputSystem()
    {
        playerControls ??= new PlayerControls();
        SubscribeToInputEvents();
    }

    private void SubscribeToInputEvents()
    {
        playerControls.Player.Move.performed += OnMove;
        playerControls.Player.Move.canceled += OnMove;
    }

    private void UnsubscribeFromInputEvents()
    {
        playerControls.Player.Move.performed -= OnMove;
        playerControls.Player.Move.canceled -= OnMove;
    }

    private void OnEnable()
    {
        InitializeInputSystem();
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        if (playerControls == null) return;
        
        UnsubscribeFromInputEvents();
        playerControls.Player.Disable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        /*
        * Disabled until new release
        */
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        /*
        * Disabled until new release
        */
    }

    // ==================================================
    //                      Setters
    // ==================================================
    public void SetIsSleeping(bool isSleeping)
    {
        this.isSleeping = isSleeping;
        animator.SetBool("IsSleeping", isSleeping);
    }

    public void SetIsEating(bool isEating)
    {
        this.isEating = isEating;
        animator.SetBool("IsEating", isEating);
    }

    // ==================================================
    //                      Handlers
    // ==================================================

    private void HandleStatsDecay()
    {
        if (hungerStatValue > 0 && !isEating) {
            hungerStatValue -= hungerDecayRate * Time.deltaTime;
        }

        if (sleepyStatValue > 0 && !isSleeping){
            sleepyStatValue -= sleepyDecayRate * Time.deltaTime;
        }
    }

    // ==================================================
    //                      Eating
    // ==================================================

    public IEnumerator StartEating(Transform eatPosition)
    {
        if (isEating || isSleeping) {
            yield break;
        }

        rb.velocity = Vector2.zero;
        rb.simulated = false;

        transform.position = eatPosition.position;

        SetIsEating(true);

        yield return new WaitForSeconds(eatDuration);

        SetIsEating(false);

        rb.simulated = true;

        hungerStatValue = maxStatValue;
    }

    // ==================================================
    //                      Sleeping
    // ==================================================

    public IEnumerator StartSleeping(Transform sleepPosition)
    {
        if (isEating || isSleeping) {
            yield break;
        }

        rb.velocity = Vector2.zero;
        rb.simulated = false;

        transform.position = sleepPosition.position;

        SetIsSleeping(true);

        yield return new WaitForSeconds(sleepDuration);

        SetIsSleeping(false);

        rb.simulated = true;
        
        sleepyStatValue = maxStatValue;
    }

    // ==================================================
    //                      Updates
    // ==================================================

    private void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        HandleStatsDecay();
    }

    private void FixedUpdate()
    {
        Vector2 direction = joystick.Direction.magnitude >= 0.1f ? joystick.Direction : movementInput;
        rb.velocity = direction * movementSpeed;

        if (direction.x != 0)
            spriteRenderer.flipX = direction.x > 0;
    }       
}