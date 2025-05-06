using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Sleep")]
    [SerializeField] private Animator animator;

    private SpriteRenderer spriteRenderer;
    private Vector2 movementInput;
    private Rigidbody2D rb;
    private PlayerControls playerControls;
    private bool isSleeping;
    private Coroutine sleepTimerCoroutine;
    private Transform currentBed;

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
        if (isSleeping || !other.TryGetComponent<Bed>(out var bed)) return;
        
        currentBed = bed.sleepPosition;
        StartSleepCountdown();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Bed>(out _) && sleepTimerCoroutine != null)
        {
            StopSleepCountdown();
        }
    }

    private void StartSleepCountdown()
    {
        sleepTimerCoroutine = StartCoroutine(SleepCountdown());
    }

    private void StopSleepCountdown()
    {
        if (sleepTimerCoroutine != null)
        {
            StopCoroutine(sleepTimerCoroutine);
            sleepTimerCoroutine = null;
        }
    }

    private IEnumerator SleepCountdown()
    {
        yield return new WaitForSeconds(3f);
        StartSleeping();
    }

    public void StartSleeping()
    {
        isSleeping = true;
        animator.SetBool("IsSleeping", true);
        transform.position = currentBed.position;
        rb.velocity = Vector2.zero;
        rb.simulated = false;
    }

    private void WakeUp()
    {
        isSleeping = false;
        animator.SetBool("IsSleeping", false);
        rb.simulated = true;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (isSleeping) WakeUp();
        movementInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (isSleeping) return;
        rb.velocity = movementInput * moveSpeed;

        if(movementInput.x != 0) {
            spriteRenderer.flipX = movementInput.x < 0;
        }
    }
}