using System;
using System.Runtime.CompilerServices;
using Input;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Player _player;
    [SerializeField] private Vector3 localPlayerOffset;
    [SerializeField] private Vector2 force = new Vector2(700, 1100);
    [SerializeField] private Rigidbody2D gravityRigidbody;

    public static Bomb Get()
    {
        return FindObjectOfType<Bomb>();
    }
    
    private void Awake()
    {
        _player = Player.Get();
        Physics2D.IgnoreCollision(_player.GetComponent<Collider2D>(), gravityRigidbody.GetComponent<Collider2D>());
    }

    private void DisableGravity()
    {
        gravityRigidbody.isKinematic = true;
    }
    
    private void EnableGravity()
    {
        gravityRigidbody.isKinematic = false;
    }

    public void Kick(bool isRight = false)
    {
        EnableGravity();
        UnparentPlayer();
        gravityRigidbody.velocity = Vector2.zero;
        gravityRigidbody.Sleep();
        var forceX = Mathf.Abs(force.x);
        force.x = isRight ? forceX : -forceX;
        gravityRigidbody.AddForce(force, ForceMode2D.Force);
    }

    [Button]
    public void Grab()
    {
        DisableGravity();
        ParentPlayer();
        SetOffsetRelativeToPlayer();
    }

    private void SetOffsetRelativeToPlayer()
    {
        transform.localPosition = localPlayerOffset;
    }

    private void ParentPlayer()
    {
        transform.SetParent(_player.transform);
    }

    private void UnparentPlayer()
    {
        transform.SetParent(null);
    }

    [Button]
    private void TestLeft()
    {
        Kick(true);
    }
    [Button]
    private void TestRight()
    {
        Kick();
    }
    
    protected bool IsPlayer(Collider2D other)
    {
        return other == other.CompareTag(_player.tag);
    }
    
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsPlayer(collision.collider))
        {
            
        }
    }
}
