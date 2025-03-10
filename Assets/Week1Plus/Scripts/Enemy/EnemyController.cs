using UnityEngine;
using EnumTypes;

public class EnemyController : MonoBehaviour
{
    private bool _canDamage = false;

    protected GameManager gameManager;
    public EnemyType enemyType;
    private BossEnemy _bossEnemy;
    
    protected Transform Player;
    protected Rigidbody2D Rigidbody;
    [SerializeField] private int score = 50;
    
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
    public EnemyMovement EnemyMovement;
    
    
    protected virtual void Start()
    {
        gameManager = GameManager.Instance;
        Player = gameManager.Player.gameObject.transform;
        Rigidbody = GetComponent<Rigidbody2D>();
        HealthTotalCount = healthMaxCount;
        
        EnemyMovement.InitMovement(Player, Rigidbody);

        if (enemyType is EnemyType.DashBoss or EnemyType.Laser or EnemyType.BlackHole)
        {
            _bossEnemy = GetComponent<BossEnemy>();
        }

        if (!_bossEnemy)
        {
            EnemyMovement.StartMovement(enemyType);
        }

        StartDamage(true);
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

    public bool IsBossEnemy()
    {
        return _bossEnemy;
    }
}
