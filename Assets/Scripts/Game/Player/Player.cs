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
        _dogAnimation = GetComponentInChildren<DogAnimation>();
    }

    private DogAnimation _dogAnimation;
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
        _dogAnimation.EnableBomb();
    }

    public void DropBomb()
    {
        hasBomb = false;
        _dogAnimation.DisableBomb();
    }

    public void SetIsGrounded(bool value)
    {
        isGrounded = value;
    }
}
