using EnumTypes;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool _canMove = true;

    public PlayerAttack playerAttack;
    public PlayerNotifier PlayerNotifier;
    public bool IsBossState;
    
    [Header("Health")]
    public int HealthTotalCount
    {
        get => _healthTotalCount;
        set
        {
            _healthTotalCount = value;
            GameManager.Instance.UIManager.UpdateHealth(_healthTotalCount);
            if (_healthTotalCount <= 0)
            {
                KillPlayer();
            }
        }
    }
    
    [SerializeField] private int healthMaxCount = 5;
    private int _healthTotalCount;
    
    [Header("Movement")]
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private float moveSpeed = 5f;
    
    private Vector2 _movement;
    
    private float constraintXValue = 16;
    private float constraintYValue = 10;
    private float constraintOffset = 1;

    private void Start()
    {
        HealthTotalCount = healthMaxCount;
    }

    private void Update()
    {
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.y = Input.GetAxisRaw("Vertical");

        RotatePlayer();
        MovePlayer();
        ConstraintPosition(rigidbody);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<EnemyController>();

            // Basic enemy
            if (!enemy.IsBossEnemy())
            {
                enemy.KillEnemy(false, true);
            }
            
            HealthTotalCount--;
            StartCoroutine(GameManager.Instance.Camera.ShakeCameraCo());
        }

        if (collision.CompareTag("Laser"))
        {
            HealthTotalCount--;
            StartCoroutine(GameManager.Instance.Camera.ShakeCameraCo());
        }


        if (collision.CompareTag("Item"))
        {
            var item = collision.GetComponent<Item>();
            item.UseItem();
        }
    }

    public void StartPlayerMove(bool canMove)
    {
        _canMove = canMove;
        rigidbody.linearVelocity = Vector2.zero;
    }

    private void MovePlayer()
    {
        if (!_canMove)
        {
            return;
        }

        rigidbody.linearVelocity = _movement.normalized * moveSpeed;
    }
    
    public void ConstraintPosition(Rigidbody2D rigidbody)
    {
        if (!IsBossState)
        {
            return;
        }

        if (GameManager.Instance.Camera.IsTransitioning)
        {
            return;
        }
        
        var offset = 0.04f;
        var minBounds = Camera.main.ViewportToWorldPoint(new Vector3(offset, offset, transform.position.z));
        var maxBounds = Camera.main.ViewportToWorldPoint(new Vector3(1 - offset, 1 - offset, transform.position.z));
        
        var clampedPosition = new Vector3(
            Mathf.Clamp(rigidbody.position.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(rigidbody.position.y, minBounds.y, maxBounds.y)
        );
        
        rigidbody.position = clampedPosition;
    }

    private void RotatePlayer()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        Vector2 direction = (mousePosition - transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void ChangePlayerHealth(int amount)
    {
        HealthTotalCount += amount;
    }
    

    private void KillPlayer()
    {
        GameManager.Instance.ChangeGameState(GameState.Score);
    }
}
