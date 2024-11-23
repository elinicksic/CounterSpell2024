using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    
    // Array to store your movement sprites
    public Sprite[] movementSprites;
    
    // How fast to switch between sprites (lower = faster)
    public float spriteChangeTime = 0.1f;
    
    private float timer = 0f;
    private int currentSpriteIndex = 0;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Get WASD input
        float horizontal = Input.GetAxisRaw("Horizontal"); // A & D
        float vertical = Input.GetAxisRaw("Vertical");     // W & S
        
        // Create movement vector and normalize it
        Vector2 movement = new Vector2(horizontal, vertical);
        if (movement.magnitude > 1f)
            movement.Normalize();
            
        // Move the character
        rb.linearVelocity = movement * moveSpeed;
        
        // Only cycle sprites if moving (pressing WASD)
        if (movement != Vector2.zero)
        {
            // Increment timer
            timer += Time.deltaTime;
            
            // If enough time has passed, change to next sprite
            if (timer >= spriteChangeTime)
            {
                timer = 0f; // Reset timer
                currentSpriteIndex = (currentSpriteIndex + 1) % movementSprites.Length;
                spriteRenderer.sprite = movementSprites[currentSpriteIndex];
            }
            
            // Flip sprite based on movement direction
            if (horizontal != 0)
            {
                spriteRenderer.flipX = horizontal < 0;
            }
        }
    }
}