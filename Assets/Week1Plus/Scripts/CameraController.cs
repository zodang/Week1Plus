using System.Collections;
using EnumTypes;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameManager _gameManager;
    [SerializeField] private GameObject player;
    private GameObject _target;
    
    [SerializeField] private float transitionSpeed = 2.0f;

    private Vector3 _damangeFXPosition;
    private bool _inDamage;
    
    private bool _isTransitioning;
    public bool IsTransitioning
    {
        get => _isTransitioning;
        set
        {
            _isTransitioning = value;
            _gameManager.Player.StartPlayerMove(!_isTransitioning);
        }
    }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _target = player;
    }


    public void ChangeTarget(GameObject newTarget)
    {
        _target = newTarget;
    }
    
    void Update()
    {
        // Camera Shaking 
        _damangeFXPosition = _inDamage ? new Vector3(MakeDamageFXMagnitude(), MakeDamageFXMagnitude(), 0) : Vector3.zero;

        // Follow Player
        if (_target == player)
        {
            FollowPlayer();
        }
        else
        {
            transform.position += _damangeFXPosition;
        }
    }
    
    private float MakeDamageFXMagnitude()
    {
        return Random.Range(-0.2f, 0.2f);
    }


    private void FollowPlayer()
    {
        var playerTransform = player.transform;
        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, -10) +
                             _damangeFXPosition;
    }
    
    public void SetCameraTarget(GameObject newTarget)
    {
        _target = newTarget;
        
        if (newTarget == player)
        {
            StartCoroutine(ChangeFocusToPlayer(_target.transform));

        }
        else
        {
            
            StartCoroutine(ChangeFocusToTarget(_target.transform));
        }
    }
    
    private IEnumerator ChangeFocusToTarget(Transform newTarget)
    {
        IsTransitioning = true;
        _gameManager.Player.PlayerNotifier.SetBossNotifier(false);
        
        var startPosition = transform.position;
        var targetPosition = new Vector3(newTarget.position.x, newTarget.position.y, -10);
        var t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        IsTransitioning = false;
        _gameManager.Player.IsBossState = true;
        _gameManager.StartElapsedTimer();
        
        _gameManager.SpawnManager.SpawnedBossEnemy.StartDamage(true);

    }
    
    private IEnumerator ChangeFocusToPlayer(Transform newTarget)
    {
        IsTransitioning = true;
        
        var startPosition = transform.position;
        var targetPosition = new Vector3(newTarget.position.x, newTarget.position.y, -10);
        var t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        IsTransitioning = false;
        _gameManager.ChangeGameState(GameState.Score);
    }
    
    public IEnumerator ShakeCameraCo()
    {
        _inDamage = true;
        yield return new WaitForSeconds(0.1f);
        _inDamage = false;
    }
}
