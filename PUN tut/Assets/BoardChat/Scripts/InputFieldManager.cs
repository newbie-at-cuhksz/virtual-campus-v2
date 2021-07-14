// this cript should be attached to the board input field under the setboard panel in the canvas
// this script create a static transform variable to reference the input field of the setboard panel
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    public class InputFieldManager : MonoBehaviour
    {
        public static Transform InputFieldTransform;
        void Start()
        {
            InputFieldTransform = this.transform;
        }
    }
}
