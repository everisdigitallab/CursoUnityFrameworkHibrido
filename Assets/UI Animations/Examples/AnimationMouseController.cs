using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMouseController : MonoBehaviour
{
    private float initialMousePosition;

    [SerializeField] UIAnim controlledAnimation;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            controlledAnimation.anims[0].controlled = true;
            initialMousePosition = Input.mousePosition.x;
        }

        if (Input.GetMouseButtonUp(0))
        {
            controlledAnimation.anims[0].controlled = false;
        }

        if (controlledAnimation.anims[0].controlled)
        {
            controlledAnimation.anims[0].controlledPos = (Input.mousePosition.x - initialMousePosition) / (Screen.width / 5f);
        }
    }
}
