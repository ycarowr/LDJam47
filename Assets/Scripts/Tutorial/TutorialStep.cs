using System;
using UnityEngine;

namespace Tutorial
{
    [RequireComponent(typeof(Collider2D))]
    public class TutorialStep : MonoBehaviour
    {
        public TutorialStep nextStep;
        public bool isFirst;

        private void Awake()
        {
            if(!isFirst)
                gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(nextStep != null)
                nextStep.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
