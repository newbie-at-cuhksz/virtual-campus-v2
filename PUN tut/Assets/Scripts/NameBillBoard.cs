using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NameBillBoard : MonoBehaviour
{
    public Text PlayerName;

    private GameObject mainCamera;
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        PlayerName.text = transform.parent.gameObject.GetComponent<PhotonView>().Owner.NickName;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + mainCamera.transform.forward);
    }
}
