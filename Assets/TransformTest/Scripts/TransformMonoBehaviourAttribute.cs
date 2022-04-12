using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformMonoBehaviourAttribute : MonoBehaviour
{
    [SerializeField] int iterations;

    void Update()
    {
        for (int i = 0; i < iterations; i++)
        {
            Transform trans = transform;
        }
    }
}
