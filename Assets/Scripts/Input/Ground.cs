using System.Collections;
using System.Collections.Generic;
using Input;
using UnityEngine;

public class Ground : UiBaseEntity
{
    private PlayerControls _controls;

    protected override void OnCollisionEnterPlayer()
    {
        if (_controls == null)
            _controls = Player.GetComponent<PlayerControls>();
        _controls.SetIsGrounded(true);
    }
}
