using UnityEngine;
using System.Collections;

public class SpriteMover : MonoBehaviour
{
    public float speed = 5f; // Speed at which the sprite moves
    public float startDelay = 0f; // Delay before the object starts moving

    public float minY = -5f; // Minimum Y position for randomization
    public float maxY = 5f;  // Maximum Y position for randomization

    private float startY; // Starting y position
    private float endX;   // Ending x position
    private bool isMoving = false; // Flag to indicate if the sprite is currently moving

    void Start()
    {
        // Set the ending x position to the left edge of the screen
        endX = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x - GetComponent<SpriteRenderer>().bounds.size.x / 2;

        // Start the movement after the specified delay
        StartCoroutine(StartMovementAfterDelay());
    }

    IEnumerator StartMovementAfterDelay()
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(startDelay);

        // Randomize the Y position when the movement starts
        SetRandomYPosition();

        // Start moving the object
        isMoving = true;
        while (isMoving)
        {
            // Move the sprite left
            transform.Translate(Vector3.left * speed * Time.deltaTime);

            // Check if the sprite has reached the end point
            if (transform.position.x <= endX)
            {
                // Stop moving and reset the position
                isMoving = false;
                ResetPosition();
            }

            yield return null; // Wait for the next frame
        }
    }

    void SetRandomYPosition()
    {
        // Set the starting y position to a random value within the specified range
        startY = Random.Range(minY, maxY);
        transform.position = new Vector3(transform.position.x, startY, transform.position.z);
    }

    void ResetPosition()
    {
        // Randomize the Y position only when resetting
        SetRandomYPosition();
        
        // Reset the X position to the starting point
        float startX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + GetComponent<SpriteRenderer>().bounds.size.x / 2;
        transform.position = new Vector3(startX, startY, transform.position.z);

        // Restart the movement
        StartCoroutine(StartMovementAfterDelay());
    }
}






