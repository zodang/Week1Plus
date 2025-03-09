using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BossEnemy : EnemyController
{
    [Header("Boss Enemy")]
    [SerializeField] private Slider healthSlider;
    private Vector2 _randomDirection;
    
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float rushDistance = 10f;

    protected override void Start()
    {
        base.Start();

        healthSlider.maxValue = healthMaxCount;
        healthSlider.value = healthMaxCount;
    }

    protected override void Update()
    {
        // 
    }

    public void UpdateHealthBar()
    {
        healthSlider.value = HealthTotalCount;
    }

    public IEnumerator MoveRoutine()
    {
        while (GameManager.Instance.Player.IsBossState)
        {
            yield return new WaitForSeconds(3f);
        
            for (var i = 0; i < 3; i++)
            {
                var startPos = transform.position;
                var randomDirection = Random.insideUnitCircle.normalized;
                var targetPos = startPos + (Vector3)randomDirection * rushDistance;
                
                targetPos = ClampToCameraBounds(targetPos);
                
                StartCoroutine(Move(targetPos));
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private IEnumerator Move(Vector2 targetPos)
    {
        while (Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, dashSpeed * Time.deltaTime);
            yield return null;
        }
    }
    private Vector3 ClampToCameraBounds(Vector3 targetPos)
    {
        if (Camera.main == null) return targetPos;

        var offset = 0.1f;
        var minBounds = Camera.main.ViewportToWorldPoint(new Vector3(offset, offset, transform.position.z));
        var maxBounds = Camera.main.ViewportToWorldPoint(new Vector3(1 - offset, 1 - offset, transform.position.z));

        targetPos.x = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);

        return targetPos;
    }
}
