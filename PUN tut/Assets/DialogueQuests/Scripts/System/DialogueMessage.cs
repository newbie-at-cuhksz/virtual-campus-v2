using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueQuests
{
    public enum DialogueMessageType
    {
        DialoguePanel=0, //Regular dialogue panel
        InGame=5, //In-game dialogue
    }

    public class DialogueMessage : MonoBehaviour
    {
        public ActorData actor;

        [TextArea(3, 10)]
        public string text;

        public AudioClip audio_clip = null;
        public string anim;

        [Tooltip("For in-game dialogues: time dialogue is shown")]
        public float duration = 4f;
        [Tooltip("For in-game dialogues: time of the pause between this dialogue and the next one")]
        public float pause = 0f;

        public UnityAction onStart;
        public UnityAction onEnd;

        //Get the text for display
        public string GetText()
        {
            string txt = text;

            //If you integrate a translation system, convert your string here!
            txt = NarrativeTool.Translate(txt);

            //Replace codes like [s:variable_id]
            txt = NarrativeTool.ReplaceCodes(txt);

            return txt;
        }
    }

}