using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private readonly Vector3 _negativeOne = new Vector3(-1, 1, 1);
    private readonly int _idle = Animator.StringToHash("DogIdle");
    private readonly int _walk = Animator.StringToHash("DogWalkLeft");
    private readonly int _jump = Animator.StringToHash("DogJump");
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject bombAnchor;
    [SerializeField] private GameObject trail;


    private bool _isJumping;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        DisableBomb();
    }
    
    public void WalkLeft()
    {
        if (_isJumping)
            return;
        
        _animator.Play(_walk);
        _spriteRenderer.flipX = true;
        bombAnchor.transform.localScale = _negativeOne;
        trail.transform.localScale = _negativeOne;
    }
    
    public void WalkRight()
    {
        if (_isJumping)
            return;
        
        _animator.Play(_walk);
        _spriteRenderer.flipX = false;
        bombAnchor.transform.localScale = Vector3.one;
        trail.transform.localScale = Vector3.one;
    }

    [Button]
    public void Jump()
    {
        _isJumping = true;
        _animator.Play(_jump);
    }

    [Button]
    public void Idle()
    {
        if (_isJumping)
            return;

        _animator.Play(_idle);
    }

    [Button]
    public void EnableBomb()
    {
        bombAnchor.SetActive(true);
    }
    
    [Button]
    public void DisableBomb()
    {
        bombAnchor.SetActive(false);
    }

    public void EndJump()
    {
        _isJumping = false;
    }
}
