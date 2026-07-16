using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2.0f;
    public float heightRange = 1.5f;
    
    [Tooltip("Changes when this platform starts moving relative to others")]
    public float timeOffset = 0.0f; 

    private Vector3 startPosition;
    private Rigidbody rb;

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Adding 'timeOffset' to 'Time.time' staggers the movement timing
        float newY = startPosition.y + (Mathf.Sin((Time.time + timeOffset) * speed) * heightRange);
        Vector3 targetPosition = new Vector3(transform.position.x, newY, transform.position.z);
        
        rb.MovePosition(targetPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponentInChildren<Camera>())
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponentInChildren<Camera>())
        {
            other.transform.SetParent(null);
        }
    }
}
