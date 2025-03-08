using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;
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
        }
    }
    
    private void DamageEnemy(EnemyController enemy, int damageValue)
    {
        StartParticle();
        enemy.DamageEnemy(damageValue);
        Destroy(gameObject);
    }

    public void StartParticle()
    {
        Instantiate(particle, transform.position, Quaternion.identity);
    }
}
