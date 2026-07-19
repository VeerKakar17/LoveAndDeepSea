using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] movementSprites;
    [SerializeField] private Sprite[] movementSpritesAncient;
    [SerializeField] private float secondsPerFrame = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private int currentFrame;
    private int frameDirection = 1;
    private float frameTimer;
    private bool isPlaying = true;
    private bool initialized = false;
    private Sprite[] currMovSprites;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currMovSprites = movementSprites;
    }

    private void Update()
    {
        if (!isPlaying || movementSprites == null || movementSprites.Length == 0)
        {
            return;
        }

        if (!initialized)
        {
            if (rb.linearVelocity.sqrMagnitude < 0.001f)
            {
                currentFrame = 0;
                frameDirection = 1;
                frameTimer = 0f;
                spriteRenderer.sprite = movementSprites[0];
                return;
            }
        }

        frameTimer += Time.deltaTime;

        if (frameTimer < secondsPerFrame)
        {
            return;
        }

        frameTimer -= secondsPerFrame;
        currentFrame += frameDirection;

        if (currentFrame >= currMovSprites.Length - 1)
        {
            currentFrame = currMovSprites.Length - 1;
            frameDirection = -1;
        }
        else if (currentFrame <= 0)
        {
            currentFrame = 0;
            frameDirection = 1;
        }

        spriteRenderer.sprite = currMovSprites[currentFrame];
    }
    
    public void InitMovementAnimation()
    {
        initialized = true;
        isPlaying = false;
    }

    public void StartMovementAnimation()
    {
        isPlaying = true;
    }

    public void StopMovementAnimation()
    {
        isPlaying = false;
    }

    public void SwapTime()
    {
        if (currMovSprites == movementSprites)
        {
            currMovSprites = movementSpritesAncient;
        } else
        {
            currMovSprites = movementSprites;
        }
        currentFrame = 0;
        frameDirection = 1;
        frameTimer = 0f;
    }
}