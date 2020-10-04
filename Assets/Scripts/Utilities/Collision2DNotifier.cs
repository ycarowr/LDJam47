using System;
using UnityEditor.UIElements;
using UnityEngine;

namespace Utilities
{
    [RequireComponent(typeof(Collider2D))]
    public class Collision2DNotifier : MonoBehaviour
    {
        public event EventHandler<Collision2D> OnNotifyCollision;
        public event EventHandler<Collision2D> OnNotifyLeave;

        private void OnCollisionEnter2D(Collision2D other) => OnNotifyCollision?.Invoke(this, other);
        private void OnCollisionExit2D(Collision2D other) => OnNotifyLeave?.Invoke(this, other);
    }
}