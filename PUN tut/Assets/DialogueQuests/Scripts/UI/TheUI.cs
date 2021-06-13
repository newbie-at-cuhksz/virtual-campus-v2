using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueQuests
{
    /// <summary>
    /// Main UI Canvas script
    /// </summary>

    public class TheUI : MonoBehaviour
    {

        private Canvas canvas;
        private RectTransform rect;

        private static TheUI _instance;

        void Awake()
        {
            _instance = this;
            canvas = GetComponent<Canvas>();
            rect = GetComponent<RectTransform>();
        }

        private void Start()
        {
            canvas.worldCamera = Camera.main;
        }

        void Update()
        {

        }

        public static TheUI Get()
        {
            return _instance;
        }
    }

}
