using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGetComponent : MonoBehaviour
{
    [SerializeField] int iterations;

    void Update()
    {
        for (int i = 0; i < iterations; i++)
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
        }
    }
}
