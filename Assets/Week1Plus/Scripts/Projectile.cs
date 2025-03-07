using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed = 10f;
    private Rigidbody2D _rigidbody;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.linearVelocity = transform.right * projectileSpeed;
    }
    
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
