using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace timeControl
{
    public class TimeControlRoomFor1 : MonoBehaviour
    {
        public GameObject dayWindows;
        public GameObject nightWindows;
        public GameObject dayLight;
        public float timer;
        public float speed = 0.2f;

        public Material SkyBoxNight;
        public Material SkyBoxDay;
        public Material SkyBoxDawn;
        public Material SkyBoxDusk;

        public GameObject TimeDisplay;

        private Quaternion targetRotation;

        // Start is called before the first frame update
        void Start()
        {
            timer = 5.0f;
        }

        // Update is called once per frame
        void Update()
        {
            timer = timer + Time.deltaTime * speed;
            if (timer > 24)
            {
                Debug.Log("New day!");
                timer = 0.0f;
            }
            //TimeDisplay.GetComponent<TextMesh>


            // day settings
            if (timer < 18 && timer > 6)
            {
                dayWindows.GetComponent<MeshRenderer>().enabled = true;
                nightWindows.GetComponent<MeshRenderer>().enabled = false;
                dayLight.GetComponent<Light>().enabled = true;

                targetRotation = Quaternion.Euler(36.4f,-90+((timer-6)/12.0f)*180.0f,0);

            }
            else // night settings
            {
                dayWindows.GetComponent<MeshRenderer>().enabled = false;
                nightWindows.GetComponent<MeshRenderer>().enabled = true;
                dayLight.GetComponent<Light>().enabled = false;
            }


            if(timer>=5&&timer<7)
            {
                RenderSettings.skybox= SkyBoxDawn;
            }
            else if (timer>=7 && timer<17)
            {
                RenderSettings.skybox = SkyBoxDay;
            }
            else if(timer>=17 && timer <19)
            {
                RenderSettings.skybox = SkyBoxDusk;
            }
            else
            {
                RenderSettings.skybox = SkyBoxNight;
            }
        }

        private void FixedUpdate()
        {
            dayLight.transform.rotation = Quaternion.Slerp(dayLight.transform.rotation, targetRotation, Time.deltaTime);
        }

    }
}
