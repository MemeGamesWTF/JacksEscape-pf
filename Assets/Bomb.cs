using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float speed = 10f; // Speed of the bomb movement
    private Vector2 movementDirection; // Random movement direction

    void Start()
    {
        // Generate a random direction for movement (both X and Y axes)
        movementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    void Update()
    {
        // Move the bomb in the random direction
        transform.Translate(movementDirection * speed * Time.deltaTime);

        // Deactivate the bomb if it goes out of bounds
        if (Mathf.Abs(transform.position.x) > 20f || Mathf.Abs(transform.position.y) > 20f)
        {
            gameObject.SetActive(false);
        }
    }
}
