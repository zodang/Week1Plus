using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private GameObject particle;
    private Rigidbody2D _rigidbody;

    protected virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.linearVelocity = transform.right * speed;
    }
    
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            DamageEnemy(enemy, damage);
            Destroy(gameObject);
        }
    }
    
    private void DamageEnemy(EnemyController enemy, float damageValue)
    {
        StartParticle();
        enemy.DamageEnemy(damageValue);
    }

    public void StartParticle()
    {
        Instantiate(particle, transform.position, Quaternion.identity);
    }
}
