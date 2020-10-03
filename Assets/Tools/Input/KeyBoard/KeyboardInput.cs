﻿using System;
using UnityEngine;

public interface IKeyboardInput
{
    bool IsTracking { get; }
    KeyCode Key { get; }
    Action OnKey { get; set; }
    Action OnKeyDown { get; set; }
    Action OnKeyUp { get; set; }
    void StartTracking();
    void StopTracking();
}

public class KeyboardInput : MonoBehaviour, IKeyboardInput
{
    [SerializeField] KeyCode key;
    public bool IsTracking { get; private set; }
    KeyCode IKeyboardInput.Key => key;

    public Action OnKey { get; set; } = () => { };
    public Action OnKeyDown { get; set; } = () => { };
    public Action OnKeyUp { get; set; } = () => { };

    public void StartTracking() => IsTracking = true;

    public void StopTracking() => IsTracking = false;

    void Update()
    {
        if (!IsTracking)
            return;

        var isKey = UnityEngine.Input.GetKey(key);
        var isKeyDown = UnityEngine.Input.GetKeyDown(key);
        var isKeyUp = UnityEngine.Input.GetKeyUp(key);

        if (isKey)
            OnKey?.Invoke();
        if (isKeyDown)
            OnKeyDown?.Invoke();
        if (isKeyUp)
            OnKeyUp?.Invoke();
    }

    public void SetKey(KeyCode keyCode) => key = keyCode;
}
