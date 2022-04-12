using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformCustomAttribute : MonoBehaviour
{
    Transform myTransform;
    [SerializeField] int iterations;

    private void Awake()
    {
        myTransform = transform;
    }

    void Update()
    {
        for (int i = 0; i < iterations; i++)
        {
            Transform trans = myTransform;
        }
    }
}
