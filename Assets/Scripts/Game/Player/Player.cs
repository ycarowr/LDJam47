using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Action OnDie = () => { };

    public static Player Get()
    {
        return FindObjectOfType<Player>();
    }

    public bool isDead;

    public void Die()
    {
        isDead = true;
        OnDie.Invoke();
    }
}
