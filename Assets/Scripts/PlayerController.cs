using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject ghostCharacter; // Reference to red character
    public float delayTime = 5f;
    
    // Sprites for blue character
    public Sprite[] blueSprites;
    // Sprites for red character
    public Sprite[] redSprites;
    
    public float spriteChangeTime = 0.1f;
    private float spriteTimer = 0f;
    private int currentSpriteIndex = 0;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer ghostSprite;
    
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
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (ghostCharacter != null)
        {
            ghostSprite = ghostCharacter.GetComponent<SpriteRenderer>();
        }
        
        // Set initial sprites
        if (blueSprites != null && blueSprites.Length > 0)
        {
            spriteRenderer.sprite = blueSprites[0];
        }
        if (ghostSprite != null && redSprites != null && redSprites.Length > 0)
        {
            ghostSprite.sprite = redSprites[0];
        }
    }

    void Update()
    {
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
    }
    
    void UpdateGhost()
    {
        if (ghostCharacter == null || positionHistory.Count == 0 || redSprites == null || redSprites.Length == 0) 
            return;
        
        float targetTime = Time.time - delayTime;
        
        // Find the historical position closest to our target time
        for (int i = 0; i < positionHistory.Count; i++)
        {
            if (positionHistory[i].TimeStamp >= targetTime)
            {
                // Update ghost position and state
                ghostCharacter.transform.position = positionHistory[i].Position;
                ghostSprite.flipX = positionHistory[i].IsFacingLeft;
                
                // Use the red sprites for the ghost, matching the animation frame
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
}

// using UnityEngine;
// using System.Collections.Generic;

// public class PlayerController : MonoBehaviour
// {
//     public float moveSpeed = 5f;
//     public GameObject ghostCharacter; // Reference to red character
//     public float delayTime = 5f; // Delay in seconds
    
//     // Animation settings
//     public Sprite[] movementSprites;
//     public float spriteChangeTime = 0.1f;
//     private float spriteTimer = 0f;
//     private int currentSpriteIndex = 0;
    
//     private Rigidbody2D rb;
//     private SpriteRenderer spriteRenderer;
    
//     // Store position, sprite, and state history
//     private List<CharacterState> positionHistory = new List<CharacterState>();
    
//     // Class to store state at each frame
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
        
//         // Set initial sprite
//         if (movementSprites != null && movementSprites.Length > 0)
//         {
//             spriteRenderer.sprite = movementSprites[0];
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
//         if (movement != Vector2.zero && movementSprites != null && movementSprites.Length > 0)
//         {
//             spriteTimer += Time.deltaTime;
//             if (spriteTimer >= spriteChangeTime)
//             {
//                 spriteTimer = 0f;
//                 currentSpriteIndex = (currentSpriteIndex + 1) % movementSprites.Length;
//                 spriteRenderer.sprite = movementSprites[currentSpriteIndex];
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
        
//         // Update ghost character
//         UpdateGhost();
        
//         // Clean up old history entries
//         CleanHistory();
//     }
    
//     void UpdateGhost()
//     {
//         if (ghostCharacter == null || positionHistory.Count == 0) return;
        
//         SpriteRenderer ghostSprite = ghostCharacter.GetComponent<SpriteRenderer>();
//         if (ghostSprite == null) return;
        
//         float targetTime = Time.time - delayTime;
        
//         // Find the historical position closest to our target time
//         for (int i = 0; i < positionHistory.Count; i++)
//         {
//             if (positionHistory[i].TimeStamp >= targetTime)
//             {
//                 // Update ghost position and state
//                 ghostCharacter.transform.position = positionHistory[i].Position;
//                 ghostSprite.flipX = positionHistory[i].IsFacingLeft;
                
//                 // Update ghost sprite if we have sprites available
//                 if (movementSprites != null && movementSprites.Length > 0)
//                 {
//                     ghostSprite.sprite = movementSprites[positionHistory[i].SpriteIndex];
//                 }
//                 break;
//             }
//         }
//     }
    
//     void CleanHistory()
//     {
//         // Remove positions older than our delay
//         float removeBeforeTime = Time.time - delayTime - 1f;
//         positionHistory.RemoveAll(x => x.TimeStamp < removeBeforeTime);
//     }
// }



