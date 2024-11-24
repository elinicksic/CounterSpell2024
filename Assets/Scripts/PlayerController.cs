using UnityEngine;
using System.Collections.Generic;
using TMPro; // For TextMeshPro

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 5f;
    public GameObject ghostCharacter;
    public float delayTime = 5f;

    // UI elements for dialogue
    public GameObject dialoguePanel; // Assign the panel GameObject
    public TMP_Text dialogueText; // Changed to TMP_Text
    public string catchMessage = "You're so sus!!!";


    // Sprites for both characters
    public Sprite[] blueSprites;
    public Sprite[] redSprites;

    public float spriteChangeTime = 0.1f;
    private float spriteTimer = 0f;
    private int currentSpriteIndex = 0;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer ghostSprite;
    private bool isGamePaused = false;
    private float gameStartTime;
    private float graceTime = 5f; // 5 seconds grace period

    public AudioClip clip;

    private AudioSource source;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private List<CharacterState> positionHistory = new List<CharacterState>();

    private class CharacterState
    {
        public Vector2 Position;
        public bool IsFacingLeft;
        public int SpriteIndex;
        public float TimeStamp;

        public CharacterState(Vector2 pos, bool facingLeft, int spriteIndex, float time)
        {
            Position = pos;
            IsFacingLeft = facingLeft;
            SpriteIndex = spriteIndex;
            TimeStamp = time;
        }
    }

    void Start()
    {
        source = GetComponent<AudioSource>();

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameStartTime = Time.time;

        if (ghostCharacter != null)
        {
            ghostSprite = ghostCharacter.GetComponent<SpriteRenderer>();
        }

        // Explicitly hide dialogue panel at start
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
            Debug.Log("Dialogue panel hidden at start"); // Debug log
        }
        else
        {
            Debug.LogWarning("Dialogue Panel is not assigned!"); // Warning if panel is missing
        }

        // Set initial sprites
        if (blueSprites != null && blueSprites.Length > 0)
            spriteRenderer.sprite = blueSprites[0];
        if (ghostSprite != null && redSprites != null && redSprites.Length > 0)
            ghostSprite.sprite = redSprites[0];
    }

    void Update()
    {
        if (isGamePaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Handle movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical);
        if (movement.magnitude > 1f)
            movement.Normalize();

        rb.linearVelocity = movement * moveSpeed;

        // Handle sprite animation
        if (movement != Vector2.zero && blueSprites != null && blueSprites.Length > 0)
        {
            spriteTimer += Time.deltaTime;
            if (spriteTimer >= spriteChangeTime)
            {
                spriteTimer = 0f;
                currentSpriteIndex = (currentSpriteIndex + 1) % blueSprites.Length;
                spriteRenderer.sprite = blueSprites[currentSpriteIndex];
            }
        }

        // Handle sprite flipping
        if (horizontal != 0)
        {
            spriteRenderer.flipX = horizontal < 0;
        }

        // Record current position and state
        positionHistory.Add(new CharacterState(
            transform.position,
            spriteRenderer.flipX,
            currentSpriteIndex,
            Time.time
        ));

        UpdateGhost();
        CleanHistory();

        // Only check collision after grace period
        if (Time.time - gameStartTime > graceTime)
        {
            CheckCollision();
        }
    }

    void UpdateGhost()
    {
        if (ghostCharacter == null || positionHistory.Count == 0 || redSprites == null || redSprites.Length == 0)
            return;

        float targetTime = Time.time - delayTime;

        for (int i = 0; i < positionHistory.Count; i++)
        {
            if (positionHistory[i].TimeStamp >= targetTime)
            {
                ghostCharacter.transform.position = positionHistory[i].Position;
                ghostSprite.flipX = positionHistory[i].IsFacingLeft;

                int ghostSpriteIndex = positionHistory[i].SpriteIndex % redSprites.Length;
                ghostSprite.sprite = redSprites[ghostSpriteIndex];
                break;
            }
        }
    }

    void CleanHistory()
    {
        float removeBeforeTime = Time.time - delayTime - 1f;
        positionHistory.RemoveAll(x => x.TimeStamp < removeBeforeTime);
    }

    void CheckCollision()
    {
        if (ghostCharacter == null) return;

        // Check distance between characters
        float distance = Vector2.Distance(transform.position, ghostCharacter.transform.position);
        if (distance < 0.5f) // Adjust this value based on your sprite sizes
        {
            source.PlayOneShot(clip);
            ShowDialogue();
        }
    }

    void ShowDialogue()
    {
        isGamePaused = true;
        rb.linearVelocity = Vector2.zero; // Stop movement

        if (dialoguePanel != null)
        {
            Debug.Log("Showing dialogue panel"); // Debug log
            dialoguePanel.SetActive(true);
            if (dialogueText != null)
            {
                dialogueText.text = catchMessage;
                Debug.Log("Set dialogue text: " + catchMessage); // Debug log
            }
            else
            {
                Debug.LogWarning("Dialogue Text component is missing!"); // Warning if text is missing
            }
        }
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }
}


// using UnityEngine;
// using System.Collections.Generic;

// public class PlayerController : MonoBehaviour
// {
//     public float moveSpeed = 5f;
//     public GameObject ghostCharacter; // Reference to red character
//     public float delayTime = 5f;

//     // Sprites for blue character
//     public Sprite[] blueSprites;
//     // Sprites for red character
//     public Sprite[] redSprites;

//     public float spriteChangeTime = 0.1f;
//     private float spriteTimer = 0f;
//     private int currentSpriteIndex = 0;

//     private Rigidbody2D rb;
//     private SpriteRenderer spriteRenderer;
//     private SpriteRenderer ghostSprite;

//     private List<CharacterState> positionHistory = new List<CharacterState>();

//     private class CharacterState
//     {
//         public Vector2 Position;
//         public bool IsFacingLeft;
//         public int SpriteIndex;
//         public float TimeStamp;

//         public CharacterState(Vector2 pos, bool facingLeft, int spriteIndex, float time)
//         {
//             Position = pos;
//             IsFacingLeft = facingLeft;
//             SpriteIndex = spriteIndex;
//             TimeStamp = time;
//         }
//     }

//     void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         spriteRenderer = GetComponent<SpriteRenderer>();

//         if (ghostCharacter != null)
//         {
//             ghostSprite = ghostCharacter.GetComponent<SpriteRenderer>();
//         }

//         // Set initial sprites
//         if (blueSprites != null && blueSprites.Length > 0)
//         {
//             spriteRenderer.sprite = blueSprites[0];
//         }
//         if (ghostSprite != null && redSprites != null && redSprites.Length > 0)
//         {
//             ghostSprite.sprite = redSprites[0];
//         }
//     }

//     void Update()
//     {
//         // Handle movement
//         float horizontal = Input.GetAxisRaw("Horizontal");
//         float vertical = Input.GetAxisRaw("Vertical");

//         Vector2 movement = new Vector2(horizontal, vertical);
//         if (movement.magnitude > 1f)
//             movement.Normalize();

//         rb.linearVelocity = movement * moveSpeed;

//         // Handle sprite animation
//         if (movement != Vector2.zero && blueSprites != null && blueSprites.Length > 0)
//         {
//             spriteTimer += Time.deltaTime;
//             if (spriteTimer >= spriteChangeTime)
//             {
//                 spriteTimer = 0f;
//                 currentSpriteIndex = (currentSpriteIndex + 1) % blueSprites.Length;
//                 spriteRenderer.sprite = blueSprites[currentSpriteIndex];
//             }
//         }

//         // Handle sprite flipping
//         if (horizontal != 0)
//         {
//             spriteRenderer.flipX = horizontal < 0;
//         }

//         // Record current position and state
//         positionHistory.Add(new CharacterState(
//             transform.position,
//             spriteRenderer.flipX,
//             currentSpriteIndex,
//             Time.time
//         ));

//         UpdateGhost();
//         CleanHistory();
//     }

//     void UpdateGhost()
//     {
//         if (ghostCharacter == null || positionHistory.Count == 0 || redSprites == null || redSprites.Length == 0)
//             return;

//         float targetTime = Time.time - delayTime;

//         // Find the historical position closest to our target time
//         for (int i = 0; i < positionHistory.Count; i++)
//         {
//             if (positionHistory[i].TimeStamp >= targetTime)
//             {
//                 // Update ghost position and state
//                 ghostCharacter.transform.position = positionHistory[i].Position;
//                 ghostSprite.flipX = positionHistory[i].IsFacingLeft;

//                 // Use the red sprites for the ghost, matching the animation frame
//                 int ghostSpriteIndex = positionHistory[i].SpriteIndex % redSprites.Length;
//                 ghostSprite.sprite = redSprites[ghostSpriteIndex];
//                 break;
//             }
//         }
//     }

//     void CleanHistory()
//     {
//         float removeBeforeTime = Time.time - delayTime - 1f;
//         positionHistory.RemoveAll(x => x.TimeStamp < removeBeforeTime);
//     }
// }

