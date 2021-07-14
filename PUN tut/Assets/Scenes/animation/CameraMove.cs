using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CamAnimation
{

    public class CameraMove : MonoBehaviour
    {
        public float forwardSpeed;          //前进的速度
        public float backwardSpeed;         //后退的速度
        public float rotateSpeed;           //旋转速度
        private Vector3 velocity;

        void FixedUpdate()
        {
            //获取到横轴 前后 的输入 也就是键盘W 和S的输入
            float h = Input.GetAxis("Horizontal");
            //获取到纵轴 左右 的输入 也就是键盘A 和D的输入
            float v = Input.GetAxis("Vertical");
            //从上下键的输入，获取到Z轴的输入量
            velocity = new Vector3(0, 0, v);
            //将世界坐标转化为本地坐标
            velocity = transform.TransformDirection(velocity);
            //判断是前进还是后退
            if (v > 0.1)
            {
                velocity *= forwardSpeed;
            }
            else
            {
                velocity *= backwardSpeed;
            }
            //移动自身坐标
            transform.localPosition += velocity * Time.fixedDeltaTime;
            //旋转角度
            transform.Rotate(0, h * rotateSpeed, 0);
        }
    }

}