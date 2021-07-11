using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectImageButtonManager : MonoBehaviour
{
    public static SelectImageButtonManager instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void Activate()
    {
        this.gameObject.SetActive(true);
    }

    public void HideButton()
    {
        this.gameObject.SetActive(false);
    }
}
