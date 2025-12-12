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

    // Tambahan untuk behavior "hold to auto-jump on landing" seperti Geometry Dash
    private bool jumpHeld;
    private bool prevIsGrounded;

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

        // Ketahui apakah tombol lompat sedang ditekan (hold)
        jumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.W);

        // Input Lompat: saat ditekan pertama kali (jump) dan berada di tanah
        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            jumpRequest = true; 
        }

        // Jika pemain "menahan" tombol lompat dan baru saja mendarat (landing),
        // buat lagi request lompat sehingga otomatis melompat lagi seperti Geometry Dash.
        if (isGrounded && jumpHeld && !prevIsGrounded)
        {
            jumpRequest = true;
        }

        prevIsGrounded = isGrounded;
    }

    // Fisika (langsung set velocity agar bergerak konstan)
    void FixedUpdate()
    {
        if (rb.isKinematic) return;

        MovePlayer();

        if (jumpRequest)
        {
            Jump();
            jumpRequest = false;
        }
    }

    void MovePlayer()
    {
        // Saat ada input horizontal, langsung set kecepatan horizontal konstan
        // (tidak menurun saat melompat)
        float targetSpeed = hInput * moveSpeed;

        if (Mathf.Abs(hInput) > 0.01f)
        {
            rb.linearVelocity = new Vector3(targetSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
        }
        else
        {
            // Tidak ada input: di tanah berhenti segera (feeling Mario),
            // di udara biarkan tetap bergerak.
            if (isGrounded)
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, rb.linearVelocity.z);
        }

        // Pastikan tidak melebihi maxSpeed (safety)
        if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
        {
            float limitedSpeed = Mathf.Sign(rb.linearVelocity.x) * maxSpeed;
            rb.linearVelocity = new Vector3(limitedSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
        }
    }

    void Jump()
    {
        // Reset komponen vertikal saja sebelum lompat
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}