using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Weapon Settings")]
    [SerializeField] private Transform gunPivot;
    private float initialGunPosX;
    private bool isDead = false;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Camera mainCam;

    private Vector2 moveInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCam = Camera.main;

        if (gunPivot != null)
        {
            initialGunPosX = gunPivot.localPosition.x;
        }
    }

    public void DisableController() => isDead = true;

    private void Update()
    {
        if (isDead) return;

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        animator.SetBool("isMoving", moveInput != Vector2.zero);
        HandleSpriteRotation();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleSpriteRotation()
    {
        if (mainCam == null) return;

        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        if (mousePos.x < transform.position.x)
        {
            spriteRenderer.flipX = true;

            if (gunPivot != null)
            {
                gunPivot.localPosition = new Vector3(-initialGunPosX, gunPivot.localPosition.y, 0);
            }
        }
        else if (mousePos.x > transform.position.x)
        {
            spriteRenderer.flipX = false;

            if (gunPivot != null)
            {
                gunPivot.localPosition = new Vector3(initialGunPosX, gunPivot.localPosition.y, 0);
            }
        }
    }
}