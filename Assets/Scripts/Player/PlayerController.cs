using System.Collections; 
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Joystick joystick;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    public Transform foodTransform;
    public Transform bedTransform;

    private bool isEating = false;
    private bool isSleeping = false;
    private bool isDancing = false;
    private bool isWaiting = false;
    private bool isMovingAutomatically = false;

    private Vector3 normalScale;
    private float idleTimer = 0f;
    private float waitingThreshold = 5f;
    private float nearBedTimer = 0f;
    private float nearBedThreshold = 3f;
    private Coroutine currentCoroutine;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        normalScale = transform.localScale;
    }

    void Update()
    {
        if (isMovingAutomatically || isEating || isSleeping || isDancing)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 input = new Vector2(joystick.Horizontal, joystick.Vertical);


        if (input.magnitude < 0.1f)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= waitingThreshold && !isWaiting)
            {
                isWaiting = true;
                animator.SetBool("IsWaiting", true);
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsIdle", false);
            }

            float distToBed = Vector2.Distance(transform.position, bedTransform.position);
            if (distToBed <= 1.5f)
            {
                nearBedTimer += Time.deltaTime;
                if (nearBedTimer >= nearBedThreshold && !isSleeping)
                {
                    if (currentCoroutine != null) StopCoroutine(currentCoroutine);
                    currentCoroutine = StartCoroutine(SleepRoutine());
                }
            }
            else
            {
                nearBedTimer = 0f;
            }
        }
        else
        {
            if (isWaiting)
            {
                isWaiting = false;
                animator.SetBool("IsWaiting", false);
            }

            idleTimer = 0f;
            nearBedTimer = 0f;
        }

        if (input.magnitude > 0.1f)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsIdle", false);
            spriteRenderer.flipX = input.x < 0;
        }
        else if (!isWaiting)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsIdle", true);
        }
    }

    private void FixedUpdate()
    {
        if (isSleeping || isMovingAutomatically || isEating || isDancing) return;

        Vector2 input = new Vector2(joystick.Horizontal, joystick.Vertical);

        rb.velocity = input * moveSpeed;

        if (input.x != 0)
        {
            spriteRenderer.flipX = input.x < 0;
        }

        Debug.Log("Joystick Input: " + joystick.Horizontal + ", " + joystick.Vertical);

    }

    public void Sleep()
    {
        if (!isSleeping && !isEating && !isDancing)
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);

            Transform sleepPos = bedTransform.Find("SleepPosition");
            if (sleepPos != null)
            {
                currentCoroutine = StartCoroutine(MoveToAndSleep(sleepPos.position));
            }
            else
            {
                Debug.LogError("SleepPosition tidak ditemukan di dalam Bed GameObject");
            }
        }
    }


    public void Eat()
    {
        if (!isSleeping && !isEating && !isDancing)
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);

            Transform eatPos = foodTransform.Find("EatPosition");
            if (eatPos != null)
            {
                currentCoroutine = StartCoroutine(MoveToAndEat(eatPos.position));
            }
            else
            {
                Debug.LogError("EatPosition tidak ditemukan di dalam FoodPlate GameObject");
            }
        }
    }

    public void Dance()
    {
        if (!isSleeping && !isEating && !isDancing)
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(DanceRoutine());
        }
    }

    private IEnumerator MoveToAndSleep(Vector3 targetPosition)
    {
        isMovingAutomatically = true;

        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector2 direction = (targetPosition - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsIdle", false);
            spriteRenderer.flipX = direction.x < 0;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        isMovingAutomatically = false;

        currentCoroutine = StartCoroutine(SleepRoutine());
    }

    private IEnumerator MoveToAndEat(Vector3 targetPosition)
    {
        isMovingAutomatically = true;

        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector2 direction = (targetPosition - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsIdle", false);
            spriteRenderer.flipX = direction.x < 0;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        isMovingAutomatically = false;

        currentCoroutine = StartCoroutine(EatRoutine());
    }

    private IEnumerator EatRoutine()
    {
        isEating = true;
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsIdle", false);
        animator.SetTrigger("Eat");

        yield return new WaitForSeconds(5f);

        isEating = false;
        animator.SetBool("IsIdle", true);
    }

    private IEnumerator DanceRoutine()
    {
        isDancing = true;
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsIdle", false);
        animator.SetTrigger("Dance");

        yield return new WaitForSeconds(5f);

        isDancing = false;
        animator.SetBool("IsIdle", true);
    }

    private IEnumerator SleepRoutine()
    {
        isSleeping = true;
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsIdle", false);
        animator.SetTrigger("Sleep");

        transform.localScale = new Vector3(normalScale.x, -Mathf.Abs(normalScale.y), normalScale.z);
        rb.simulated = false;

        yield return new WaitForSeconds(5f);

        isSleeping = false;
        animator.SetBool("IsIdle", true);
        transform.localScale = normalScale;
        rb.simulated = true;
    }

    private void CancelCurrentAction()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        if (isEating || isDancing || isSleeping)
        {
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsWalking", false);
            animator.ResetTrigger("Eat");
            animator.ResetTrigger("Dance");
            animator.ResetTrigger("Sleep");

            transform.localScale = normalScale;
            rb.simulated = true;

            isEating = false;
            isSleeping = false;
            isDancing = false;
            isMovingAutomatically = false;
        }
    }

    // ======= Tambahan fungsi untuk ubah posisi food dan bed =======
    public void SetFoodPosition(Vector3 newFoodPosition)
    {
        if (foodTransform == null)
        {
            GameObject foodObj = new GameObject("FoodPosition");
            foodTransform = foodObj.transform;
        }
        foodTransform.position = newFoodPosition;
        Debug.Log("Food position set to: " + newFoodPosition);
    }

    public void SetBedPosition(Vector3 newBedPosition)
    {
        if (bedTransform == null)
        {
            GameObject bedObj = new GameObject("BedPosition");
            bedTransform = bedObj.transform;
        }
        bedTransform.position = newBedPosition;
        Debug.Log("Bed position set to: " + newBedPosition);
    }
}
