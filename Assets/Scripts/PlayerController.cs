using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private SpriteRenderer spriteRenderer;
    private Vector2 movementInput;
    private Rigidbody2D rb;
    private PlayerControls playerControls;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
        playerControls.Player.Move.performed += OnMove;
        playerControls.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
        playerControls.Player.Move.performed -= OnMove;
        playerControls.Player.Move.canceled -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.velocity = movementInput * moveSpeed;

        if(movementInput.x != 0) {
            spriteRenderer.flipX = movementInput.x < 0;
        }
    }
}