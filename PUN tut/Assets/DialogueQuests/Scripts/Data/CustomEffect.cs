using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueQuests
{
    /// <summary>
    /// Base class for creating custom effects, inherit from this class, add the [CreateAssetMenu()] tag
    /// And then create a data file based on this script, the DoEffect() function will be called when resolving the effect
    /// </summary>
    /// 
    public class CustomEffect : ScriptableObject
    {
        [System.NonSerialized]
        public bool inited = false;

        //Override this function to init parameters at start
        public virtual void Start() { }

        //Override this function in your custom effect
        // player property could be null if the event wasnt triggered by any player (ex: AtStart event)
        public virtual void DoEffect(Actor player) {
            
        }
    }

}
