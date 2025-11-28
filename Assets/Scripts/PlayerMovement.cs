using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Pengaturan Gerak")]
    public float moveSpeed = 10f;
    public float maxSpeed = 15f;       // Batas kecepatan
    public float airMultiplier = 0.1f; 
    public float jumpForce = 7f;

    [Header("Cek Tanah")]
    public float groundDist = 0.6f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded;
    private float hInput;
    private bool jumpRequest;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Input Kiri/Kanan
        hInput = Input.GetAxis("Horizontal");

        // Cek Tanah
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundDist, groundLayer);
    
        Debug.DrawRay(transform.position, Vector3.down * groundDist, isGrounded ? Color.green : Color.red);

        // Input Lompat (Spasi ATAU W)
        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            jumpRequest = true; 
        }
    }

    // Fisika (AddForce) agar stabil
    void FixedUpdate()
    {
        MovePlayer();

        if (jumpRequest)
        {
            Jump();
            jumpRequest = false;
        }
    }

    void MovePlayer()
    {
        float currentForce = isGrounded ? moveSpeed : moveSpeed * airMultiplier;

        rb.AddForce(Vector3.right * hInput * currentForce);

        if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
        {
            float limitedSpeed = Mathf.Sign(rb.linearVelocity.x) * maxSpeed;
            rb.linearVelocity = new Vector3(limitedSpeed, rb.linearVelocity.y, rb.linearVelocity.z);

        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, 0);

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}