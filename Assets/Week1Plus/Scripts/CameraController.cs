using System.Collections;
using EnumTypes;
using UnityEngine;

public class CameraController : MonoBehaviour
{
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
            GameManager.Instance.Player.ActivatePlayerMovement(!_isTransitioning);
        }
    }
    
    private float MakeDamageFXMagnitude(){
        return Random.Range(-0.2f, 0.2f);
    }


    void Start()
    {
        _target = player;
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
        GameManager.Instance.Player.PlayerNotifier.SetBossNotifier(false);
        
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
        GameManager.Instance.Player.IsBossState = true;
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
        GameManager.Instance.ChangeGameState(GameState.Score);
    }
    
    public IEnumerator ShakeCameraCo()
    {
        _inDamage = true;
        yield return new WaitForSeconds(0.1f);
        _inDamage = false;
    }
}
