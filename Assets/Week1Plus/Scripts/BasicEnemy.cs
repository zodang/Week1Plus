using UnityEngine;

public class BasicEnemy : EnemyController
{
    [SerializeField] private float chaseRange = 5f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
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
}
