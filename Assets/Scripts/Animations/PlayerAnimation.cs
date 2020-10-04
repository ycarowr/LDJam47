using Tools.Trail;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private readonly Vector3 _negativeOne = new Vector3(-1, 1, 1);
    
    private readonly int _idle = Animator.StringToHash("HeroIdle");
    private readonly int _walk = Animator.StringToHash("HeroRun");
    private readonly int _jump = Animator.StringToHash("HeroJump");
    private readonly int _fall = Animator.StringToHash("HeroFall");

    [SerializeField] private GameObject trail;
    
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private TrailParticles _trailDashAnim;

    private void Awake()
    {
        _trailDashAnim = GetComponentInChildren<TrailParticles>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    public void RunLeft()
    {
        _animator.Play(_walk);
        _spriteRenderer.flipX = true;
        trail.transform.localScale = _negativeOne;
    }

    public void RunRight()
    {
        _animator.Play(_walk);
        _spriteRenderer.flipX = false;
        trail.transform.localScale = Vector3.one;
    }

    [Button]
    public void Jump()
    {
        _animator.Play(_jump);
    }

    [Button]
    public void Idle()
    {
        _animator.Play(_idle);
    }

    public void Fall()
    {
        _animator.Play(_fall);
    }

    public void Dash()
    {
        _trailDashAnim.Play();
    }

    public void StopDash()
    {
        _trailDashAnim.StopTrail();
    }

    public bool IsLookingRight()
    {
        return !_spriteRenderer.flipX;
    }
}
