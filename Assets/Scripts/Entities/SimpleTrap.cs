using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTrap : Entity
{
    protected override void OnTriggerEnter2DPlayer()
    {
        Debug.Log($"On Trigger Enter 2D Player");
    }
}
