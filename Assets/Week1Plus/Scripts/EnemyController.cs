using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Transform Player;
    
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
    
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float chaseRange = 5f;

    [SerializeField] protected Rigidbody2D Rigidbody;
    [SerializeField] private ParticleSystem collisionParticles;
    
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        ChasePlayer();
    }

    private void ChasePlayer()
    {
        var distance = Vector2.Distance(Player.position, transform.position);
        if (distance > chaseRange)
        {
            Rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = Player.position - transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        Rigidbody.linearVelocity = direction.normalized * moveSpeed;
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
