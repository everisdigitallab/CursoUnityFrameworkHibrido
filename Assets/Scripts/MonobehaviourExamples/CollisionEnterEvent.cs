using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEnterEvent : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float velocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.velocity = Vector3.right * velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("Collision");
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Trigger");
    }
}
