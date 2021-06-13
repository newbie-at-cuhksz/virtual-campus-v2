using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueQuests
{
    [CreateAssetMenu(fileName = "Actor", menuName = "DialogueQuests/Actor", order = 0)]
    public class ActorData : ScriptableObject
    {
        public string actor_id;

        [Tooltip("Is the player character")]
        public bool is_player = false;

        [Header("Character name")]
        public string title;
        [Tooltip("Portrait Image")]
        public Sprite portrait;
        [Tooltip("Animated Portrait (Optional)")]
        public RuntimeAnimatorController animation;

        [Header("Chat Bubble")]
        public bool show_bubble;
        public Vector3 bubble_offset;
        public float bubble_size = 1f;

        [Header("Global Dialogues")]
        [Tooltip("Dialogue prefab that should be loaded in all scenes")]
        public GameObject load_dialogue;

        public string GetTitle()
        {
            return NarrativeTool.Translate(title);
        }

        public static void Load(ActorData actor)
        {
            if (NarrativeManager.Get())
            {
                List<ActorData> list = GetAll();
                if (!list.Contains(actor))
                {
                    list.Add(actor);
                }
            }
        }

        public static ActorData Get(string actor_id)
        {
            if (NarrativeManager.Get())
            {
                foreach (ActorData actor in GetAll())
                {
                    if (actor.actor_id == actor_id)
                        return actor;
                }
            }
            return null;
        }

        public static List<ActorData> GetAll()
        {
            return NarrativeManager.Get().actor_list;
        }
    }

}
