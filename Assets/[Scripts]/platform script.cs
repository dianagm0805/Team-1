using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2.0f;
    public float heightRange = 1.5f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the smooth up and down oscillation using a Sine wave
        float newY = startPosition.y + (Mathf.Sin(Time.time * speed) * heightRange);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    // When the VR player lands on the platform, make them a child of it
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponentInChildren<Camera>())
        {
            other.transform.SetParent(transform);
        }
    }

    // When the VR player jumps off, unparent them so they don't inherit its movement
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponentInChildren<Camera>())
        {
            other.transform.SetParent(null);
            
            // If your XR Origin is part of a persistent scene manager, 
            // you can explicitly unparent it to root by setting:
            // DontDestroyOnLoad(other.gameObject); 
        }
    }
}
