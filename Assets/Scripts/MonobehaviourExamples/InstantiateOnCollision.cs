using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateOnCollision : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float velocity;

    [SerializeField] Vector3 positionToInstantiate;

    [SerializeField] GameObject fireworksPrefab;

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
        Instantiate(gameObject, positionToInstantiate, transform.rotation);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(fireworksPrefab, transform.position, transform.rotation);
    }
}
