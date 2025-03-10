using UnityEngine;
using UnityEngine.UI;

public class BossEnemy : EnemyController
{
    [Header("Boss Enemy")]
    [SerializeField] private Slider healthSlider;
    private Vector2 _randomDirection;

    protected override void Start()
    {
        base.Start();

        healthSlider.maxValue = healthMaxCount;
        healthSlider.value = healthMaxCount;
        
        gameManager.Player.PlayerNotifier.SetBossObject(gameObject);
        StartDamage(false);
    }

    public void UpdateHealthBar()
    {
        healthSlider.value = HealthTotalCount;
    }
}
