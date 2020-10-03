using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTrap : MonoBehaviour
{
    private UiPressEButton _buttonE;
    
    private void Start()
    {
        _buttonE = GetComponentInChildren<UiPressEButton>();
        _buttonE.AddListener(DoSomething);
    }

    private void DoSomething()
    {
        Debug.Log("Start Processing Simple Trap");
    }
}
