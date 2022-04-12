using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionChangeScene : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float velocity;
    [SerializeField] string sceneToChange;

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
        SceneManager.LoadScene(sceneToChange);
    }
}
