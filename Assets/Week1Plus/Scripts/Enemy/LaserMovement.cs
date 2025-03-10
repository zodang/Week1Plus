using UnityEngine;

public class LaserMovement : MonoBehaviour
{
    private enum LaserState { Idle, Growing, Active, Shrinking }
    private LaserState currentState = LaserState.Idle;

    private float laserElapsed = 0f;
    [SerializeField] private GameObject laserPrefab;
    private GameObject laser;
    private Vector2 targetDirection;
    private float laserDistance = 0f;
    
    [SerializeField] private float growSpeed = 3.0f;
    [SerializeField] private float laserDuration = 1.5f;
    [SerializeField] private float maxLaserDistance = 20f;

    [SerializeField] private float laserRate = 3f;
    [SerializeField] private float laserRange = 8f;
    private bool _canFire = true;

    public void StartLaserMovement(bool canFire)
    {
        _canFire = canFire;
    }
    
    private void Update()
    {
        if (_canFire && currentState == LaserState.Idle)
        {
            StartFiring();
        }

        switch (currentState)
        {
            case LaserState.Growing:
                laserElapsed += Time.deltaTime * growSpeed;
                laserDistance = Mathf.Lerp(0, maxLaserDistance, laserElapsed);
                UpdateLaserTransform();

                if (laserElapsed >= 1f)
                {
                    laserElapsed = 0f;
                    currentState = LaserState.Active;
                    Invoke(nameof(StartShrinking), laserDuration);
                }
                break;

            case LaserState.Shrinking:
                laserElapsed += Time.deltaTime * growSpeed;
                laserDistance = Mathf.Lerp(maxLaserDistance, 0, laserElapsed);
                UpdateLaserTransform();

                if (laserElapsed >= 1f)
                {
                    Destroy(laser);
                    laser = null;
                    _canFire = false;
                    Invoke(nameof(ResetFire), laserRate);
                    currentState = LaserState.Idle;
                }
                break;
        }
    }

    private void StartFiring()
    {
        _canFire = false;
        currentState = LaserState.Growing;
        laserElapsed = 0f;

        Vector2 targetPosition = GameManager.Instance.Player.transform.position;
        targetDirection = (targetPosition - (Vector2)transform.position).normalized;
        laserDistance = 0f;

        // Create laser
        laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        laser.transform.localScale = new Vector3(0.5f, 0f, 1f);
    }

    private void StartShrinking()
    {
        laserElapsed = 0f;
        currentState = LaserState.Shrinking;
    }

    private void ResetFire()
    {
        _canFire = true;
    }
    
    void UpdateLaserTransform()
    {
        if (laser != null)
        {
            laser.transform.localScale = new Vector3(0.5f, laserDistance, 1f);

            laser.transform.position = transform.position + (Vector3)targetDirection * (laserDistance * 0.5f);

            var angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            laser.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }
    
    private void OnDestroy()
    {
        if (laser != null)
        {
            Destroy(laser);
        }
    }
}
