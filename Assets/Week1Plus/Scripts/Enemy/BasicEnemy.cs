using UnityEngine;

public class BasicEnemy : EnemyController
{
    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (!Player)
        {
            return;
        }

        // EnemyMovement.ChasePlayer();
    }
}
