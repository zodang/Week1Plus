using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAttack : MonoBehaviour
{
    [Header("projectile")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate;
    [SerializeField] private float coolDown;
    [SerializeField] private float damage = 5;
    [SerializeField] private float projectileSpeed = 7;
    
    [Header("Misile")]
    [SerializeField] private GameObject misilePrefab;
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

        if (!Input.GetMouseButtonUp(1)) return;
        if (_currentProjectile != null)
        {
            LaunchProjectile();
        }
    }
    
    private IEnumerator ShootProjectile()
    {
        while (true)
        {
            Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
            yield return new WaitForSeconds(fireRate);
        }
    }
    
    private void SpawnProjectile()
    {
        if (_currentProjectile == null)
        {
            // gm.missileAmount -= 1;
            // UIManager.instance.UpdateMissile();
            Vector3 spawnPos = transform.position + Vector3.up * spawnDistance;

            _currentProjectile = Instantiate(misilePrefab, spawnPos, Quaternion.identity);
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
        // _currentProjectile.tag = "Missile Projectile";

        var direction = (_currentProjectile.transform.position - transform.position).normalized;
        var rb = _currentProjectile.GetComponent<Rigidbody2D>();
        _currentProjectile.transform.rotation = Quaternion.Euler(direction);

        rb.linearVelocity = direction * launchSpeed;


        // Init current projectile
        _currentProjectile = null;
        _rotateAngle = 90;
        _rotateDirection = 0;
    }
}
