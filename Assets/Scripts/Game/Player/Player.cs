using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Action OnDie = () => { };

    public static Player Get()
    {
        return FindObjectOfType<Player>();
    }

    private void Awake()
    {
        _playerAnimation = GetComponentInChildren<PlayerAnimation>();
    }

    private PlayerAnimation _playerAnimation;
    public bool isDead;
    public bool hasBomb;
    public bool isGrounded;

    public void Die()
    {
        isDead = true;
        OnDie.Invoke();
    }

    public void GrabBomb()
    {
        hasBomb = true;
        _playerAnimation.EnableBomb();
    }

    public void DropBomb()
    {
        hasBomb = false;
        _playerAnimation.DisableBomb();
    }

    public void SetIsGrounded(bool value)
    {
        isGrounded = value;
    }
}
