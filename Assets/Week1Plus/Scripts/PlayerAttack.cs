using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAttack : MonoBehaviour
{
    private bool _canShoot = false;
    
    [Header("projectile")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate;
    [SerializeField] private float coolDown;
    [SerializeField] private float damage = 5;
    [SerializeField] private float projectileSpeed = 7;
    
    [Header("Misile")]
    public int UltimateTotalCount = 10;
    [SerializeField] private GameObject ultimatePrefab;
    [SerializeField] private float spawnDistance;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float launchSpeed = 5f;
    private GameObject _currentProjectile;
    private float _rotateAngle = 90f;
    private int _rotateDirection = 0;
    
    private void Start()
    {
        StartCoroutine(ShootProjectile());
    }

    private void Update()
    {
        if (!_canShoot)
        {
            return;
        }

        if (UltimateTotalCount <= 0)
        {
            return;
        }
        
        if (Input.GetMouseButton(1))
        {
            if (_currentProjectile != null)
            {
                RotateProjectile();
            }
        }

        if (Input.GetMouseButtonDown(1) && _currentProjectile == null)
        {
            {
                _rotateDirection = -1;
                SpawnProjectile();
            }
        }

        if (!Input.GetMouseButtonUp(1))
        {
            return;
        }

        if (_currentProjectile != null)
        {
            LaunchProjectile();
            UltimateTotalCount--;
            GameManager.Instance.UIManager.UpdateUltimate(UltimateTotalCount);
        }
    }

    public void StartPlayerShoot(bool can)
    {
        _canShoot = can;
    }
    
    private IEnumerator ShootProjectile()
    {
        
        while (true)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) ||
                Input.GetKey(KeyCode.D))
            {
                Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
            }

            yield return new WaitForSeconds(fireRate);
        }
    }
    
    private void SpawnProjectile()
    {
        if (_currentProjectile == null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * spawnDistance;

            _currentProjectile = Instantiate(ultimatePrefab, spawnPos, Quaternion.identity);
            _currentProjectile.tag = "Untagged";
        }
    }
    
    private void RotateProjectile()
    {
        _rotateAngle += rotationSpeed * _rotateDirection * Time.deltaTime;
        var radian = _rotateAngle * Mathf.Deg2Rad;
        var newPos = transform.position + new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0) * spawnDistance;
        _currentProjectile.transform.position = newPos;
    }
    
    private void LaunchProjectile()
    {
        _currentProjectile.tag = "Ultimate";

        var direction = (_currentProjectile.transform.position - transform.position).normalized;
        var rb = _currentProjectile.GetComponent<Rigidbody2D>();
        _currentProjectile.transform.rotation = Quaternion.Euler(direction);

        rb.linearVelocity = direction * launchSpeed;

        // Init current projectile
        _currentProjectile = null;
        _rotateAngle = 90;
        _rotateDirection = 0;
    }

    public bool CheckUltimateDistance()
    {
        if (_currentProjectile == null)
        {
            return true;
        }

        return spawnDistance > Vector2.Distance(_currentProjectile.transform.position, transform.position);
    }
    
    public void ChangePlayerUltimateCount(int amount)
    {
        UltimateTotalCount += amount;
        GameManager.Instance.UIManager.UpdateUltimate(UltimateTotalCount);
    }
}
