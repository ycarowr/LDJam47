using Input;
using UnityEngine;

public abstract class UiBaseEntity : MonoBehaviour
{
    protected Rigidbody2D Rigidbody2D { get; set; }
    protected Collider Collider { get; set; }
    protected SpriteRenderer SpriteRenderer { get; set; }
    protected Animator Animator { get; set; }
    protected PlayerControls Player;

    protected virtual void Awake()
    {
        Player = PlayerControls.Get();
        Rigidbody2D = GetComponentInChildren<Rigidbody2D>();
        Collider = GetComponentInChildren<Collider>();
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Animator = GetComponentInChildren<Animator>();
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsPlayer(collision.collider))
            OnCollisionEnterPlayer();
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (IsPlayer(collision.collider))
            OnCollisionStayPlayer();
    }

    protected void OnCollisionExit2D(Collision2D collision)
    {
        if (IsPlayer(collision.collider))
            OnCollisionExitPlayer();
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayer(collision))
            OnTriggerEnterPlayer();
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (IsPlayer(collision))
            OnTriggerExitPlayer();
    }

    protected bool IsPlayer(Collider2D other)
    {
        return other == other.CompareTag(Player.tag);
    }

    protected virtual void OnCollisionEnterPlayer()
    {
    }

    protected virtual void OnCollisionStayPlayer()
    {
    }

    protected virtual void OnCollisionExitPlayer()
    {
    }

    protected virtual void OnTriggerEnterPlayer()
    {
    }

    protected virtual void OnTriggerExitPlayer()
    {
    }
}