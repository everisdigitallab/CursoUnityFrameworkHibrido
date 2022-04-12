using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastLayermask : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] float maxDistance;
    [SerializeField] LayerMask layerToClick;
    [SerializeField] Vector2 minPosition;
    [SerializeField] Vector2 maxPosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, maxDistance, layerToClick))
            {
                Transform objectHit = hit.transform;
                objectHit.transform.position = new Vector3
                    (
                    Random.Range(minPosition.x, maxPosition.x),
                    Random.Range(minPosition.y, maxPosition.y),
                    0
                    );
            }
        }
    }
}
