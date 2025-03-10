using System;
using System.Collections;
using EnumTypes;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    private EnemyType _type;
    public float moveSpeed = 5f;
    [SerializeField] protected float rotationSpeed = 8f;
    [SerializeField] private float moveTime = 0.7f;
    [SerializeField] private float restTime = 1f;
    private Vector2 _moveDirection;
    private bool _isMoving = false;
    
    [Header("Chase")]
    [SerializeField] private float basicChaseRange = 5f;
    
    [Header("Dash")] 
    [SerializeField] private int dashDelayCount = 3;
    [SerializeField] private int dashCount = 3;
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float rushDistance = 10f;
    
    private Transform _player;
    private Rigidbody2D _rigidbody;

    public void InitMovement(Transform player, Rigidbody2D rigidbody)
    {
        _player = player;
        _rigidbody = rigidbody;
    }

    public void StartMovement(EnemyType type)
    {
        _type = type;
        
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

        switch (_type)
        {
            case EnemyType.None:
                break;
            case EnemyType.Basic:
                StartCoroutine(BasicMoveRoutine());
                break;
            case EnemyType.DashBasic:
                StartCoroutine(BasicDashMoveCo());
                break;
            case EnemyType.DashBoss:
                
                StartCoroutine(DashMoveCo());
                break;
            case EnemyType.BlackHole:
                break;
            case EnemyType.Laser:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void FixedUpdate()
    {
        if (_isMoving)
        {
            _rigidbody.linearVelocity = _moveDirection * moveSpeed;
        }
    }
    
    private Vector3 ClampToCameraBounds(Vector3 targetPos)
    {
        var offset = 0.1f;
        var minBounds = Camera.main.ViewportToWorldPoint(new Vector3(offset, offset, transform.position.z));
        var maxBounds = Camera.main.ViewportToWorldPoint(new Vector3(1 - offset, 1 - offset, transform.position.z));

        targetPos.x = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);

        return targetPos;
    }


    #region Basic Enemy
    public IEnumerator BasicMoveRoutine()
    {
        while (true)
        {
            _moveDirection = Random.insideUnitCircle.normalized;
            yield return StartCoroutine(RotateTowardsDirection(_moveDirection));

            _isMoving = true;
            yield return new WaitForSeconds(moveTime);

            _isMoving = false;
            _rigidbody.linearVelocity = Vector2.zero;
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
    
    public void ChasePlayer()
    {
        var distance = Vector2.Distance(_player.position, transform.position);
        if (distance > basicChaseRange)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = _player.position - transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        _rigidbody.linearVelocity = direction.normalized * moveSpeed;
    }

    #endregion

    #region DashBasic

    public IEnumerator BasicDashMoveCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(dashDelayCount);
        
            for (var i = 0; i < dashCount; i++)
            {
                if (this == null)
                {
                    yield break;
                }

                var limitBoundary = GameManager.Instance.SpawnManager.SpawnedBossEnemy;
                var targetPos = CalculateDashPosition(limitBoundary);
                StartCoroutine(Move(targetPos));
                
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    #endregion

    #region DashBoss
    public IEnumerator DashMoveCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(dashDelayCount);
            GameManager.Instance.SpawnManager.SpawnEnemy(1, GameManager.Instance.SpawnManager.SetRandomPosition(5, 7));
            
            for (var i = 0; i < dashCount; i++)
            {
                if (this == null)
                {
                    yield break;
                }

                var targetPos = CalculateDashPosition(true);
                StartCoroutine(Move(targetPos));
                
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public Vector3 CalculateDashPosition(bool isLimitBoundary)
    {
        var startPos = transform.position;
        var randomDirection = Random.insideUnitCircle.normalized;
        var targetPos = startPos + (Vector3)randomDirection * rushDistance;
        
        if (isLimitBoundary)
        {
            targetPos = ClampToCameraBounds(targetPos);
        }

        return targetPos;
    }

    private IEnumerator Move(Vector2 targetPos)
    {
        while (Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, dashSpeed * Time.deltaTime);
            yield return null;
        }
    }

    #endregion
    
}
