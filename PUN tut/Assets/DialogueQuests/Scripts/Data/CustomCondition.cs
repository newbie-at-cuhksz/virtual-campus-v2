using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueQuests
{
    /// <summary>
    /// Base class for creating custom conditions, inherit from this class, add the [CreateAssetMenu()] tag
    /// And then create a data file based on this script, the IsMet() function will be called when checking the condition
    /// </summary>

    public class CustomCondition : ScriptableObject
    {
        [System.NonSerialized]
        public bool inited = false;

        //Override this function to init parameters at start
        public virtual void Start() { }

        //Override this function in your custom condition
        // player property could be null if the event wasnt triggered by any player (ex: AtStart event)
        public virtual bool IsMet(Actor player) {
            return true; //Default value
        }
    }

}
