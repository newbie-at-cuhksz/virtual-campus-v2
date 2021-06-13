using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueQuests.Demo
{
    /// <summary>
    /// Zone to change scene when you enter this zone, make sure there is also a trigger collider
    /// </summary>

    public class ExitZoneDemo : MonoBehaviour
    {
        [Header("Exit")]
        public string scene;

        private float timer = 0f;
        private bool transition = false;

        void Update()
        {
            timer += Time.deltaTime;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (!transition && timer > 1f && collision.GetComponent<PlayerCharacterDemo>())
            {
                if (SceneNav.DoSceneExist(scene))
                {
                    transition = true;
                    SceneNav.GoTo(scene);
                }
                else
                {
                    Debug.Log("Scene don't exist: " + scene);
                }
            }
        }

    }

}
