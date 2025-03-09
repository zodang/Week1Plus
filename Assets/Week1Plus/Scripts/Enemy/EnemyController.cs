using System.Collections;
using UnityEngine;
using EnumTypes;

public class EnemyController : MonoBehaviour
{
    private bool _canDamage = false;

    protected GameManager gameManager;
    [SerializeField] private EnemyType enemyType;
    private BossEnemy _bossEnemy;
    
    protected Transform Player;
    protected Rigidbody2D Rigidbody;
    [SerializeField] private int score = 1;
    
    [Header("Health")]
    public int HealthTotalCount
    {
        get => _healthTotalCount;
        set
        {
            _healthTotalCount = value;
            if (_healthTotalCount <= 0)
            {
                if (!_bossEnemy)
                {
                    KillEnemy(true, true);
                }
                else
                {
                    gameManager.isBossSpawned = false;
                    gameManager.Player.IsBossState = false;
                    gameManager.Camera.SetCameraTarget(gameManager.Player.gameObject);
                    KillEnemy(false, true);
                }
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
        gameManager = GameManager.Instance;
        Rigidbody = GetComponent<Rigidbody2D>();
        HealthTotalCount = healthMaxCount;
        
        if (enemyType is EnemyType.Dash or EnemyType.Laser or EnemyType.BlackHole)
        {
            _bossEnemy = GetComponent<BossEnemy>();
        }
        
        if (!_bossEnemy)
        {
            StartCoroutine(MoveRoutine());
        }
        
        StartDamage(true);
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

    public void StartDamage(bool can)
    {
        _canDamage = can;
    }
    
    public void DamageEnemy(int damage)
    {
        if (!_canDamage)
        {
            return;
        }
        
        HealthTotalCount -= damage;

        if (_bossEnemy)
        {
            _bossEnemy.UpdateHealthBar();
        }
    }

    public void KillEnemy(bool isSpawnable, bool isAbleToGet)
    {
        if (isSpawnable)
        {
            gameManager.SpawnManager.SpawnItem(transform);
        }
        gameManager.SpawnManager.RemoveSpawnedEnemy(this);

        if (isAbleToGet)
        {
            gameManager.IncreaseScore(score);
        }
        Destroy(gameObject);
    }
    
    public void AttackPlayer()
    {
        
    }

    public bool IsBossEnemy()
    {
        return _bossEnemy;
    }
}
