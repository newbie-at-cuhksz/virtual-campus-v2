using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    private GameObject joyStick;
    private FloatingJoystick floatingJoystick;
    private CharacterController characterController;
    private Vector3 moveDirection;
    
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public float gravity=20f;
    public bool isWalking = false;

    void Start()
    {
        GameObject mainCanvas = GameObject.Find("Canvas");
        joyStick = mainCanvas.transform.Find("Floating Joystick").gameObject;
        floatingJoystick = joyStick.GetComponent<FloatingJoystick>();
        characterController = transform.GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {   
        float x = floatingJoystick.Horizontal;
        float z = floatingJoystick.Vertical;
        if (characterController.isGrounded)
        {
            moveDirection = new Vector3(x, 0, z);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= moveSpeed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);

    }
}
