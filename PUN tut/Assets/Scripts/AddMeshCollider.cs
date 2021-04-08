using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMeshCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ModelAddMeshCollider();
    }

    void ModelAddMeshCollider()
    {
        foreach (var render in transform.GetComponentsInChildren<MeshRenderer>())
        {
            if (render.gameObject.GetComponent<MeshCollider>() == null){
                render.gameObject.AddComponent<MeshCollider>();
            }
            
        }
    }
}
