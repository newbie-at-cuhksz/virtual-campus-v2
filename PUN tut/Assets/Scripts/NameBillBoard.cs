using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NameBillBoard : MonoBehaviour
{
    public Text name;
    private GameObject thisPlayer;
    private GameObject mainCamera;
    void Start()
    {
        thisPlayer = transform.parent.gameObject;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        name.text = thisPlayer.GetComponent<PhotonView>().Owner.NickName;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + mainCamera.transform.forward);
    }
}
