using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueQuests;

namespace DialogueQuests.Demo
{

    public class DemoUI : MonoBehaviour
    {
        void Start()
        {

        }

        public void OnClickJournal()
        {
            QuestPanel.Get().Show();
        }
    }
}
