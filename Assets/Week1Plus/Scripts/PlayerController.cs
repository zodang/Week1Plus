using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public PlayerAttack playerAttack;
    
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
    private int _healthTotalCount = 5;

    
    [Header("Movement")]
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float constraintXValue;
    [SerializeField] private float constraintYValue;
    [SerializeField] private float constraintOffset;
    
    private Vector2 _movement;

    private bool _canMove = true;

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
        ConstraintPosition();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<EnemyController>();
            
            HealthTotalCount--;
            enemy.KillEnemy();
            StartCoroutine(GameManager.Instance.Camera.ShakeCameraCo());
        }

        if (collision.CompareTag("Item"))
        {
            var item = collision.GetComponent<Item>();
            item.UseItem();
        }
    }

    public void ActivatePlayerMovement(bool canMove)
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

    private void ConstraintPosition()
    {
        if (!GameManager.Instance.isBossState)
        {
            return;
        }

        var camera = Camera.main.transform;
        var clampedPosition = new Vector3(
            Mathf.Clamp(rigidbody.position.x, camera.position.x - (constraintXValue + constraintOffset),
                camera.position.x + constraintXValue - constraintOffset),
            Mathf.Clamp(rigidbody.position.y, camera.position.y - constraintYValue + constraintOffset,
                camera.position.x + constraintYValue - constraintOffset)
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
        throw new System.NotImplementedException();
    }
}
