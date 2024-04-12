using UnityEngine;

public class MarkerFallScript : MonoBehaviour
{
    void Update()
    {
        if(transform.position.y < 0){
            Destroy(this);
        }
    }

    private Rigidbody rb;

    void Start()
    {
        // Cache the Rigidbody component at start to optimize performance
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Table"))
        {
            // Stop the object by setting velocity and angular velocity to zero
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Move the object to the point of collision
            // Note: collision.contacts is an array, you might want to handle all points or a specific one
            var contactPoint = collision.contacts[0].point;
            transform.position = contactPoint;

            // Optionally, make the Rigidbody kinematic if you want to fully stop all physics interactions
            rb.isKinematic = true;

        }
        if (collision.transform.name == "TestFloor")
        {
            Destroy(gameObject); // Destroy the object if it hits the floor
        }
    }
}
