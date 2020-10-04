using System;
using System.Runtime.CompilerServices;
using Input;
using Tools.Trail;
using UnityEngine;
using Utilities;

public class Bomb : MonoBehaviour
{
    private PlayerControls _player;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private Vector3 localPlayerOffset;
    [SerializeField] private Vector2 force = new Vector2(1000, 1000);
    [SerializeField] private Rigidbody2D gravityRigidbody;
    private Trigger2DNotifier _groundCheck;
    private int _groundLayer;

    public static Bomb Get()
    {
        return FindObjectOfType<Bomb>();
    }
    
    private void Awake()
    {
        _groundLayer = LayerMask.NameToLayer("Ground");
        _groundCheck = GetComponentInChildren<Trigger2DNotifier>();
        _groundCheck.OnNotifyCollision += StopBounciness;
        _player = PlayerControls.Get();
        Physics2D.IgnoreCollision(_player.GetComponent<Collider2D>(), gravityRigidbody.GetComponent<Collider2D>());
        ResetPosition();
    }

    private void StopBounciness(object sender, Collider2D coll)
    {
        if (coll.gameObject.layer == _groundLayer)
            ResetVelocity();
    }

    private void ResetVelocity()
    {
        gravityRigidbody.velocity = Vector2.zero;
    }

    private void DisableGravity()
    {
        gravityRigidbody.isKinematic = true;
    }
    
    private void EnableGravity()
    {
        gravityRigidbody.isKinematic = false;
    }

    public void Kick(Vector2 direction)
    {
        EnableGravity();
        UnparentPlayer();
        gravityRigidbody.velocity = Vector2.zero;
        var effectiveForce = direction * force;
        gravityRigidbody.AddForce(effectiveForce, ForceMode2D.Force);
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
    public void ResetPosition()
    {
        gravityRigidbody.Sleep();
        ResetVelocity();
        UnparentPlayer();
        transform.position = respawnPoint.position;
        EnableGravity();
    }
}
