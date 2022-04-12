using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Test : MonoBehaviour
{
    [SerializeField] int iterations;
    [SerializeField] bool useCustomProperty;

    Vector3 customVec = new Vector3(0,0,0);

    void Update()
    {
        for (int i = 0; i < iterations; i++)
        {
            if (useCustomProperty)
            {
                customVec.Set(1, 1, 1);
                float m = customVec.magnitude;
            }
            else
            {
                float m = Vector3.one.magnitude;
            }
        }
    }
}
