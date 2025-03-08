using System.Collections;
using UnityEngine;
using EnumTypes;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyType enemyType;
    private BossEnemy _bossEnemy;
    
    protected Transform Player;
    protected Rigidbody2D Rigidbody;
    
    [Header("Health")]
    public int HealthTotalCount
    {
        
        get => _healthTotalCount;
        set
        {
            _healthTotalCount = value;
            if (_healthTotalCount <= 0)
            {
                KillEnemy();
            }
        }
    }
    
    [SerializeField] protected int healthMaxCount = 1;
    private int _healthTotalCount;
    
    [Header("Movement")]
    [SerializeField] protected float rotationSpeed = 1f;
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] private float moveTime = 2f;
    [SerializeField] private float restTime = 1f;
    private Vector2 _moveDirection;
    private bool _isMoving = false;
    
    protected virtual void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();

        if (enemyType is EnemyType.Dash or EnemyType.Laser or EnemyType.BlackHole)
        {
            _bossEnemy = GetComponent<BossEnemy>();
        }
        
        HealthTotalCount = healthMaxCount;
        StartCoroutine(MoveRoutine());
    }
    
    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            _moveDirection = Random.insideUnitCircle.normalized;
            yield return StartCoroutine(RotateTowardsDirection(_moveDirection));

            _isMoving = true;
            yield return new WaitForSeconds(moveTime);

            _isMoving = false;
            Rigidbody.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(restTime);
        }
    }
    
    private IEnumerator RotateTowardsDirection(Vector2 direction)
    {
        var targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        var targetRotation = Quaternion.Euler(0, 0, targetAngle - 90);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    protected virtual void Update()
    {
        // override
    }
    
    private void FixedUpdate()
    {
        if (_isMoving)
        {
            Rigidbody.linearVelocity = _moveDirection * moveSpeed;
        }
    }
    
    
    public void DamageEnemy(int damage)
    {
        HealthTotalCount -= damage;

        if (_bossEnemy)
        {
            _bossEnemy.UpdateHealthBar();
        }
    }

    public void KillEnemy()
    {
        GameManager.Instance.SpawnManager.SpawnItem(transform);
        Destroy(gameObject);
    }
    
    public void AttackPlayer()
    {
        
    }
}
