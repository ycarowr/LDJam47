using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Player Player;
    private Collider2D _collider2D;
    
    
    protected virtual void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        _collider2D.isTrigger = true;
        Player = Player.Get();
    }

    private bool IsPlayer(Component other)
    {
        return other.CompareTag(Player.tag);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var isPlayer = IsPlayer(other); 
        if (isPlayer) 
            OnTriggerEnter2DPlayer();
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        var isPlayer = IsPlayer(other); 
        if (isPlayer) 
            OnTriggerExit2DPlayer();
    }

    protected virtual void OnTriggerEnter2DPlayer()
    {
        
    }
    
    protected virtual void OnTriggerExit2DPlayer()
    {
        
    }
}
