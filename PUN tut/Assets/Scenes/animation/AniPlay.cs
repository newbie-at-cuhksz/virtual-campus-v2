using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniPlay : MonoBehaviour
{
    private GameObject go;
    int clipCount;
    // Start is called before the first frame update
    void Start()
    {
        go = this.gameObject;
        clipCount = go.GetComponent<Animation>().GetClipCount();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
