using UnityEngine;
using System.Collections;

namespace GercStudio.USK.Scripts
{

    public class DestroyObject : MonoBehaviour
    {
        public float DestroyTime;

        void Start()
        {
            StartCoroutine("CheckIfAlive");
        }

        IEnumerator CheckIfAlive()
        {
            while (true)
            {
                yield return new WaitForSeconds(DestroyTime);
                Destroy(gameObject);
                StopCoroutine("CheckIfAlive");
            }
        }
    }
}




