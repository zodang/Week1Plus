using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerAttack playerAttack;
    
    [Header("Movement")]
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float constraintXValue;
    [SerializeField] private float constraintYValue;
    [SerializeField] private float constraintOffset;
    
    private Vector2 _movement;

    private bool _canMove = true;

    private void Update()
    {
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.y = Input.GetAxisRaw("Vertical");

        RotatePlayer();
        MovePlayer();
        ConstraintPosition();
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
}
