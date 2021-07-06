using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CamAnimation
{

    public class CameraMove : MonoBehaviour
    {
        // Start is called before the first frame update

        public float speed=10;

        // Update is called once per frame
        void Update()
        {
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            transform.position += input * (speed * Time.deltaTime);
        }
    }

}