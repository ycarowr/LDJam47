using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
        [RequireComponent(typeof(PlayerInput))]
        public class InputBroadcaster : MonoBehaviour
        {
                public static PlayerInput Input { private set; get; }
                private void Awake() => Input = GetComponent<PlayerInput>();
        }
}
