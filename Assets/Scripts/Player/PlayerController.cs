using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public readonly float movementSpeed = 5f;
    [SerializeField] public Joystick joystick;

    [Header("Actions")]
    [SerializeField] private Animator animator;

    private SpriteRenderer spriteRenderer;
    private Vector2 movementInput;
    private Rigidbody2D rb;
    private PlayerControls playerControls;

    private FoodPlate foodPlate;
    private Bed bed;
    private Coroutine sleepCountdownCoroutine;
    private bool isSleeping;
    private bool isEating;

    // ==================================================
    //                      SETUPS
    // ==================================================

    public void SetDependencies(FoodPlate foodPlate, Bed bed)
    {
        this.foodPlate = foodPlate;
        this.bed = bed;
    }

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
        if (isSleeping || isEating) return;
        if (!other.TryGetComponent<Bed>(out _)) return;
        
        StartSleepCountdown();
        */
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        /*
        * Disabled until new release
        if (other.TryGetComponent<Bed>(out _) && sleepCountdownCoroutine != null){
            StopSleepCountdown();
        }
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
    //                      EATING
    // ==================================================

    public IEnumerator StartEating()
    {
        if (isEating || isSleeping || foodPlate == null)
        {
            yield break;
        }

        rb.velocity = Vector2.zero;
        rb.simulated = false;

        transform.position = foodPlate.eatPosition.position;

        SetIsEating(true);

        yield return new WaitForSeconds(3f);

        SetIsEating(false);

        rb.simulated = true;
    }

    // ==================================================
    //                      SLEEPING
    // ==================================================

    private void StartSleepCountdown()
    {
        sleepCountdownCoroutine = StartCoroutine(SleepCountdown());
        StartSleeping();
    }

    private void StopSleepCountdown()
    {
        if (sleepCountdownCoroutine != null) {
            StopCoroutine(sleepCountdownCoroutine);
            sleepCountdownCoroutine = null;
        }
    }

    private IEnumerator SleepCountdown()
    {
        yield return new WaitForSeconds(3f);
    }

    public void StartSleeping()
    {
        if (isEating) return;

        SetIsSleeping(true);
        transform.position = bed.sleepPosition.position;
        rb.velocity = Vector2.zero;
        rb.simulated = false;
    }

    private void StopSleeping()
    {
        if (!isSleeping) return;

        SetIsSleeping(false);
        rb.simulated = true;
    }

    // ==================================================
    //                      UPDATES
    // ==================================================

    private void OnMove(InputAction.CallbackContext context)
    {
        if (isSleeping) StopSleeping();
        movementInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (isSleeping && joystick.Direction.magnitude >= 0.1f)
            StopSleeping();

        Vector2 direction = joystick.Direction.magnitude >= 0.1f ? joystick.Direction : movementInput;
        rb.velocity = direction * movementSpeed;

        if (direction.x != 0)
            spriteRenderer.flipX = direction.x > 0;
    }       
}