using System.Collections;
using Input;
using Tools.Fade;
using UnityEngine;

public class FallDeathTrigger : MonoBehaviour
{
    private PlayerControls _player;
    private Bomb _bomb;
    private FadeComponent _fade;

    private void Awake()
    {
        _player = PlayerControls.Get();
        _bomb = Bomb.Get();
        _fade = FadeComponent.Get();
        if(_player == null)
            Debug.LogError($"Unable to find a {typeof(PlayerControls)} inside this scene.");
        if(_bomb == null)
            Debug.LogError($"Unable to find a {typeof(Bomb)} inside this scene.");
        if(_fade == null)
            Debug.LogError($"Unable to find a {typeof(FadeComponent)} inside this scene.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPlayer(other) || IsBomb(other))
            RestartLevel();
    }

    private bool IsBomb(Collider2D other)
    {
        return other.gameObject == _bomb.gameObject;
    }

    private bool IsPlayer(Collider2D other)
    {
        return other.gameObject == _player.gameObject;
    }

    private void RestartLevel()
    {
        _bomb.ResetPosition();
        _player.ResetPosition();
        _fade.FadeTo1();
        InputBroadcaster.Input.DeactivateInput();
        StartCoroutine(FadeBack());
    }

    private IEnumerator FadeBack()
    {
        yield return new WaitForSeconds(1f);
        _fade.FadeTo0();
        InputBroadcaster.Input.ActivateInput();
    }
}
