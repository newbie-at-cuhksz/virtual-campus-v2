using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueQuests
{

    public class TheLoader : MonoBehaviour
    {
        [Header("Loader")]
        public GameObject ui_canvas;
        public GameObject audio_manager;
        public GameObject event_system;
        public GameObject chat_bubble;

        [Header("Resources")]
        public string actors_folder = "Actors";
        public string quests_folder = "Quests";

        private void Awake()
        {
            if (ui_canvas != null && !FindObjectOfType<TheUI>())
                Instantiate(ui_canvas);
            if (audio_manager != null && !FindObjectOfType<TheAudio>())
                Instantiate(audio_manager);
            if (event_system != null && !FindObjectOfType<UnityEngine.EventSystems.EventSystem>())
                Instantiate(event_system);
            if (chat_bubble != null && !FindObjectOfType<ChatBubbleFX>())
                Instantiate(chat_bubble);

            ActorData[] all_actors = Resources.LoadAll<ActorData>(actors_folder);
            foreach (ActorData quest in all_actors)
                ActorData.Load(quest);

            QuestData[] all_quests = Resources.LoadAll<QuestData>(quests_folder);
            foreach (QuestData quest in all_quests)
                QuestData.Load(quest);
        }
    }

}
