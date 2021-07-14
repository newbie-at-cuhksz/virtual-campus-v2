using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageShowManager : MonoBehaviour
{

    public static Transform imagesTransform;
    // Start is called before the first frame update
    void Start()
    {
        imagesTransform = this.transform;
    }
}
