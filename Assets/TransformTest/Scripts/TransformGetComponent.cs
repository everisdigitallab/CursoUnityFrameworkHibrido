using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformGetComponent : MonoBehaviour
{
    [SerializeField] int iterations;

    void Update()
    {
        for (int i = 0; i < iterations; i++)
        {
            Transform trans = GetComponent<Transform>();
        }
    }
}
