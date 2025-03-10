using System;
using System.Collections;
using EnumTypes;
using UnityEngine;
using UnityEngine.Serialization;
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
    
    private Transform _player;
    private Rigidbody2D _rigidbody;

    public void InitMovement(EnemyType type, Transform player, Rigidbody2D rigidbody)
    {
        _type = type;
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
            case EnemyType.BasicDash:
                StartCoroutine(BasicDashMoveCo());
                break;
            case EnemyType.BasicLaser:
                StartCoroutine(BasicMoveRoutine());
                StartFiringLaser();
                break;
            case EnemyType.BasicBlackHole:
                StartCoroutine(BasicMoveRoutine());
                break;
            case EnemyType.BossDash:
                StartCoroutine(BossDashMoveCo(1));
                break;
            case EnemyType.BossLaser:
                StartFiringLaser();
                break;
            case EnemyType.BossBlackHole:
                StartCoroutine(BossDashMoveCo(3));
                SetBlackHoleMagnitude(-200);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void StartFiringLaser()
    {
        _canFire = true;
    }

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            _rigidbody.linearVelocity = _moveDirection * moveSpeed;
        }

        if (laser != null)
        {
            UpdateLaserPosition();
        }
            
        if (_type == EnemyType.BossLaser)
        {
            if (!GameManager.Instance.Player.IsBossState || !_canFire)
            {
                return;
            }
            
            if (laserCo != null)
            {
                StopCoroutine(laserCo);
            }

            laserCo = StartCoroutine(BossFireLaser());
        }

        if (_type == EnemyType.BasicLaser)
        {
            if (!_canFire)
            {
                return;
            }
            
            if (laserCo != null)
            {
                StopCoroutine(laserCo);
            }

            laserCo = StartCoroutine(BasicFireLaser());
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
    private IEnumerator BasicMoveRoutine()
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

    #region Dash
    
    [Header("Dash")] 
    [SerializeField] private int dashDelayCount = 3;
    [SerializeField] private int dashCount = 3;
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float rushDistance = 10f;

    private IEnumerator BasicDashMoveCo()
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
    
    private IEnumerator BossDashMoveCo(int basicEnemyIndex)
    {
        while (true)
        {
            yield return new WaitForSeconds(dashDelayCount);
            GameManager.Instance.SpawnManager.SpawnEnemy(basicEnemyIndex, GameManager.Instance.SpawnManager.SetRandomPosition(5, 7));
            
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
    
    #region Laser
    
    [Header("Laser")]
    [SerializeField] private GameObject laserPrefab;
    private GameObject laser;
    private Coroutine laserCo;
    
    [SerializeField] private float growSpeed = 3.0f;
    [SerializeField] private float laserDuration = 1.5f;
    [SerializeField] private float basicLaserDistance = 5f;
    [SerializeField] private float bossLaserDistance = 20f;
    
    [SerializeField] private float laserRate = 3f;
    [SerializeField] private float laserRange = 8f;
     public bool _canFire = false;
    
    private Vector2 _targetDirection;
    private float _laserDistance;
    
    private IEnumerator BasicFireLaser()
    {
        _canFire = false;

        _targetDirection = Random.insideUnitCircle.normalized;
        _laserDistance = 0f;

        // Create laser
        laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        laser.transform.localScale = new Vector3(0.5f, 0f, 1f);

        // Increase laser
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * growSpeed;
            _laserDistance = Mathf.Lerp(0, basicLaserDistance, elapsed);
            UpdateLaserTransform();
            yield return null;
        }

        yield return new WaitForSeconds(laserDuration);

        // Decrease Laser
        elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * growSpeed;
            _laserDistance = Mathf.Lerp(basicLaserDistance, 0, elapsed);
            UpdateLaserTransform();
            yield return null;
        }

        // Delete laser
        Destroy(laser);
        laser = null;

        yield return new WaitForSeconds(laserRate);
        _canFire = true;
    }
   
    private IEnumerator BossFireLaser()
    {
        _canFire = false;

        _targetDirection = Random.insideUnitCircle.normalized;
        _laserDistance = 0f;

        // Create laser
        laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        laser.transform.localScale = new Vector3(0.5f, 0f, 1f);

        // Increase laser
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * growSpeed;
            _laserDistance = Mathf.Lerp(0, bossLaserDistance, elapsed);
            UpdateLaserTransform();
            yield return null;
        }

        yield return new WaitForSeconds(laserDuration);

        // Decrease Laser
        elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * growSpeed;
            _laserDistance = Mathf.Lerp(bossLaserDistance, 0, elapsed);
            UpdateLaserTransform();
            yield return null;
        }

        // Delete laser
        Destroy(laser);
        laser = null;

        yield return new WaitForSeconds(laserRate);
        _canFire = true;
    }
    
    private void UpdateLaserTransform()
    {
        if (laser != null)
        {
            laser.transform.localScale = new Vector3(0.5f, _laserDistance, 1f);

            laser.transform.position = transform.position + (Vector3)_targetDirection * (_laserDistance * 0.5f);

            var angle = Mathf.Atan2(_targetDirection.y, _targetDirection.x) * Mathf.Rad2Deg;
            laser.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }
    
    private void UpdateLaserPosition()
    {
        if (laser != null)
        {
            laser.transform.position = transform.position + (Vector3)_targetDirection * (_laserDistance * 0.5f);
        }
    }
    #endregion

    #region BlackHole

    [Header("BlackHole")]
    [SerializeField] PointEffector2D pointEffector;
    [SerializeField] GameObject particle;

    public void SetBlackHoleMagnitude(float force)
    {
        pointEffector.forceMagnitude = force;
    }
    
    
    public void StopPointEffectorCo()
    {
        StartCoroutine(StopPointEffector(3.0f));
    }

    private IEnumerator StopPointEffector(float count)
    {
        GameManager.Instance.Player.StartPlayerDamage(true);
        SetBlackHoleMagnitude(0);
        particle.SetActive(false);
        
        yield return new WaitForSeconds(count);
        
        GameManager.Instance.Player.StartPlayerDamage(true);
        SetBlackHoleMagnitude(-200);

        particle.SetActive(true);
    }
    
    #endregion

    private void OnDestroy()
    {
        if (_type == EnemyType.BasicLaser || _type == EnemyType.BossLaser)
        {
            if (laser != null)
            {
                Destroy(laser);
            }
        }
    }
}
