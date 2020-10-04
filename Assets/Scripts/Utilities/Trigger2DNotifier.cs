using System;
using UnityEditor.UIElements;
using UnityEngine;

namespace Utilities
{
    [RequireComponent(typeof(Collider2D))]
    public class Trigger2DNotifier : MonoBehaviour
    {
        public event EventHandler<Collider2D> OnNotifyCollision;
        public event EventHandler<Collider2D> OnNotifyLeave;

        private void OnTriggerEnter2D(Collider2D other) => OnNotifyCollision?.Invoke(this, other);
        private void OnTriggerExit2D(Collider2D other) => OnNotifyLeave?.Invoke(this, other);
    }
}