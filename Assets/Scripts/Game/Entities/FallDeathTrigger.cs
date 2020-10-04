using Input;
using UnityEngine;

public class FallDeathTrigger : MonoBehaviour
{
    private PlayerControls _player;
    private Bomb _bomb;

    private void Awake()
    {
        _player = PlayerControls.Get();
        _bomb = Bomb.Get();
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
    }
}
