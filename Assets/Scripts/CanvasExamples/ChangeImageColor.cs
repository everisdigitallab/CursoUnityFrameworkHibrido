using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImageColor : MonoBehaviour
{
    Image myImage;

    void Start()
    {
        myImage = GetComponent<Image>();
    }

    public void ChangeColor()
    {
        if (myImage != null)
        {
            myImage.color = new Color(Random.value, Random.value, Random.value);
        }
    }

    
}
