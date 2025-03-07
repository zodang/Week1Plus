using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private GameObject _target;
    
    [SerializeField] private float transitionSpeed = 2.0f;
    
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


    void Start()
    {
        _target = player;
    }
    
    void Update()
    {
        if (_target == player)
        {
            FollowPlayer();
        }
    }
    
    private void FollowPlayer()
    {
        var playerTransform = player.transform;
        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, -10);
    }
    
    public void SetCameraTarget(GameObject newTarget)
    {
        _target = newTarget;
        StartCoroutine(ChangeFocusToTarget(_target.transform));
    }
    
    private IEnumerator ChangeFocusToTarget(Transform newTarget)
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
    }
}
