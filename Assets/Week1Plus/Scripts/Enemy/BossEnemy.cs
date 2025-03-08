using UnityEngine;
using UnityEngine.UI;

public class BossEnemy : EnemyController
{
    [SerializeField] private Slider healthSlider;
    protected override void Start()
    {
        base.Start();

        healthSlider.maxValue = healthMaxCount;
        healthSlider.value = healthMaxCount;
    }

    protected override void Update()
    {
        base.Update();
    }

    public void UpdateHealthBar()
    {
        healthSlider.value = HealthTotalCount;
    }
}
