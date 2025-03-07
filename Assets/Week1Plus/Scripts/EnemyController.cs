using UnityEngine;

public class EnemyController : MonoBehaviour
{
    protected Transform Player;
    protected Rigidbody2D Rigidbody;
    
    public float Health
    {
        get => health;
        set
        {
            health = value;
            if (health <= 0)
            {
                KillEnemy();
            }
        }
    }
    
    [SerializeField] private float health = 1f;
    [SerializeField] protected float moveSpeed = 1f;

    
    protected virtual void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Update()
    {
        // override
    }
    
    
    public void DamageEnemy(float damage)
    {
        Health -= damage;
    }

    private void KillEnemy()
    {
        Destroy(gameObject);
    }
    
    public void AttackPlayer()
    {
        
    }
}
